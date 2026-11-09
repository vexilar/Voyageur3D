using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Voyageur.Networking
{
    /// <summary>
    /// Server-authoritative enemy: AI + damage resolve on server; pose, anim triggers,
    /// health, and death/ragdoll replicate to clients.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkEnemyAuthority : NetworkBehaviour
    {
        enum EnemyFx : byte
        {
            Attention = 0,
            Roar = 1,
            MeleeAttack = 2,
            Hurt = 3,
            LostTarget = 4,
            Pursue = 5,
            Death = 6
        }

        [SerializeField] Animator animator;
        [SerializeField] string walkingFloatParameter = NewAgeBearAnimationStates.Walking;
        [SerializeField] float clientAnimSmooth = 12f;
        [SerializeField] float maxDamageDistance = 8f;

        readonly NetworkVariable<int> syncedHealth = new NetworkVariable<int>(
            100,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        readonly NetworkVariable<bool> isDead = new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        NavMeshAgent agent;
        EnemyHealth enemyHealth;
        EnemyRagdoll enemyRagdoll;
        NewAgeBearSoundController soundController;
        float clientWalkSpeed;
        Vector3 lastPosition;
        bool hasLastPosition;
        bool localDeathApplied;

        static readonly System.Type[] ServerOnlyBehaviourTypes =
        {
            typeof(Enemy),
            typeof(EnemyWander),
            typeof(EnemyPursue),
            typeof(EnemyMeleeAttack),
            typeof(EnemyTurnToInspect)
        };

        public bool IsNetworkDead => isDead.Value;
        public int SyncedHealth => syncedHealth.Value;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            enemyHealth = GetComponent<EnemyHealth>();
            enemyRagdoll = GetComponent<EnemyRagdoll>();
            soundController = GetComponent<NewAgeBearSoundController>();
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                if (enemyHealth != null)
                {
                    syncedHealth.Value = enemyHealth.Health;
                    enemyHealth.onEnemyHurt += HandleServerHurt;
                    enemyHealth.onEnemyDied += HandleServerDied;
                }

                BindServerPresentationEvents();
            }
            else
            {
                DisableClientSimulation();
                lastPosition = transform.position;
                hasLastPosition = true;
            }

            isDead.OnValueChanged += OnDeadChanged;
            if (isDead.Value)
            {
                ApplyLocalDeathPresentation();
            }
        }

        public override void OnNetworkDespawn()
        {
            isDead.OnValueChanged -= OnDeadChanged;

            if (IsServer && enemyHealth != null)
            {
                enemyHealth.onEnemyHurt -= HandleServerHurt;
                enemyHealth.onEnemyDied -= HandleServerDied;
                UnbindServerPresentationEvents();
            }
        }

        void BindServerPresentationEvents()
        {
            var enemy = GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.onHeardTarget += OnServerHeard;
                enemy.onSeenTarget += OnServerSeen;
                enemy.onNoLongerSuspicious += OnServerLostTarget;
            }

            var pursue = GetComponent<EnemyPursue>();
            if (pursue != null)
            {
                pursue.onPursue += OnServerPursue;
                pursue.onTargetAbandoned += OnServerLostTarget;
            }

            var melee = GetComponent<EnemyMeleeAttack>();
            if (melee != null)
            {
                melee.onMeleeAttack += OnServerMelee;
            }
        }

        void UnbindServerPresentationEvents()
        {
            var enemy = GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.onHeardTarget -= OnServerHeard;
                enemy.onSeenTarget -= OnServerSeen;
                enemy.onNoLongerSuspicious -= OnServerLostTarget;
            }

            var pursue = GetComponent<EnemyPursue>();
            if (pursue != null)
            {
                pursue.onPursue -= OnServerPursue;
                pursue.onTargetAbandoned -= OnServerLostTarget;
            }

            var melee = GetComponent<EnemyMeleeAttack>();
            if (melee != null)
            {
                melee.onMeleeAttack -= OnServerMelee;
            }
        }

        void OnServerHeard(GameObject _) => PlayFxClientRpc(EnemyFx.Attention);
        void OnServerSeen(GameObject _) => PlayFxClientRpc(EnemyFx.Roar);
        void OnServerLostTarget() => PlayFxClientRpc(EnemyFx.LostTarget);
        void OnServerPursue() => PlayFxClientRpc(EnemyFx.Pursue);
        void OnServerMelee() => PlayFxClientRpc(EnemyFx.MeleeAttack);

        void HandleServerHurt()
        {
            if (enemyHealth != null)
            {
                syncedHealth.Value = enemyHealth.Health;
            }

            PlayFxClientRpc(EnemyFx.Hurt);
        }

        void HandleServerDied()
        {
            if (enemyHealth != null)
            {
                syncedHealth.Value = Mathf.Max(0, enemyHealth.Health);
            }

            isDead.Value = true;
            PlayFxClientRpc(EnemyFx.Death);

            if (agent != null)
            {
                agent.enabled = false;
            }
        }

        void OnDeadChanged(bool previous, bool current)
        {
            if (current)
            {
                ApplyLocalDeathPresentation();
            }
        }

        void ApplyLocalDeathPresentation()
        {
            if (localDeathApplied)
            {
                return;
            }

            localDeathApplied = true;

            if (agent != null)
            {
                agent.enabled = false;
            }

            // EnemyRagdoll listens to EnemyHealth.onEnemyDied on the server.
            // Clients never get that event, so drive ragdoll directly.
            if (!IsServer && enemyRagdoll != null)
            {
                enemyRagdoll.ApplyNetworkDeath();
            }
        }

        /// <summary>
        /// Called by <see cref="EnemyHealth"/> when a networked enemy is hit.
        /// Clients forward the hit to the server; server applies authoritative damage.
        /// </summary>
        public void SubmitHit(DamageSource source, bool isCritical)
        {
            if (!IsSpawned || source == null || source.ShouldBeIgnored || isDead.Value)
            {
                return;
            }

            int damage = source.Damage;
            bool fromPlayer = source.IsFromPlayer;
            source.onHitCallBack();

            if (IsServer)
            {
                Events.invokeOnEnemyHit(gameObject, source);
                ApplyAuthoritativeDamage(damage, fromPlayer, isCritical);
                return;
            }

            RequestDamageServerRpc(damage, fromPlayer, isCritical);
        }

        [ServerRpc(RequireOwnership = false)]
        void RequestDamageServerRpc(int damage, bool fromPlayer, bool isCritical, ServerRpcParams rpcParams = default)
        {
            if (isDead.Value || damage <= 0)
            {
                return;
            }

            var sender = rpcParams.Receive.SenderClientId;
            if (NetworkManager.ConnectedClients.TryGetValue(sender, out var client) &&
                client.PlayerObject != null)
            {
                float distance = Vector3.Distance(client.PlayerObject.transform.position, transform.position);
                if (distance > maxDamageDistance)
                {
                    Debug.LogWarning(
                        $"[NetworkEnemyAuthority] Rejected damage from client {sender}: distance {distance:F1}");
                    return;
                }
            }

            ApplyAuthoritativeDamage(damage, fromPlayer, isCritical);
        }

        void ApplyAuthoritativeDamage(int damage, bool fromPlayer, bool isCritical)
        {
            if (!IsServer || enemyHealth == null || isDead.Value)
            {
                return;
            }

            enemyHealth.ApplyNetworkDamage(damage, fromPlayer, isCritical);
            syncedHealth.Value = enemyHealth.Health;
        }

        void DisableClientSimulation()
        {
            // Stop AI first so in-flight wander/pursue coroutines cannot call SetDestination
            // after the agent is disabled (common MPPM client race before spawn finishes).
            for (int i = 0; i < ServerOnlyBehaviourTypes.Length; i++)
            {
                var behaviours = GetComponentsInChildren(ServerOnlyBehaviourTypes[i], true);
                for (int j = 0; j < behaviours.Length; j++)
                {
                    if (behaviours[j] is MonoBehaviour behaviour)
                    {
                        behaviour.StopAllCoroutines();
                        behaviour.enabled = false;
                    }
                }
            }

            if (agent != null)
            {
                agent.enabled = false;
            }

            var animControllers = GetComponentsInChildren<NewAgeBearAnimationController>(true);
            for (int i = 0; i < animControllers.Length; i++)
            {
                if (animControllers[i] != null)
                {
                    animControllers[i].enabled = false;
                }
            }

            // Sounds are played from ClientRpcs so clients don't double-play server-local AI events.
            if (soundController != null)
            {
                soundController.enabled = false;
            }
        }

        [ClientRpc]
        void PlayFxClientRpc(EnemyFx fx)
        {
            // Server already plays via NewAgeBearAnimationController / SoundController.
            if (IsServer)
            {
                return;
            }

            PlayClientFx(fx);
        }

        void PlayClientFx(EnemyFx fx)
        {
            if (animator != null)
            {
                switch (fx)
                {
                    case EnemyFx.Attention:
                        animator.SetTrigger(NewAgeBearAnimationStates.Attention);
                        break;
                    case EnemyFx.Roar:
                        animator.SetTrigger(NewAgeBearAnimationStates.Roar);
                        break;
                    case EnemyFx.MeleeAttack:
                        animator.SetTrigger(NewAgeBearAnimationStates.MeleeAttack);
                        break;
                    case EnemyFx.Hurt:
                        animator.SetTrigger(NewAgeBearAnimationStates.Hurt);
                        break;
                }
            }

            if (soundController != null)
            {
                soundController.PlayNetworkFx(fx.ToString());
            }
        }

        void Update()
        {
            if (!IsSpawned || IsServer || animator == null || isDead.Value)
            {
                return;
            }

            if (!hasLastPosition)
            {
                lastPosition = transform.position;
                hasLastPosition = true;
                return;
            }

            float dt = Time.deltaTime;
            if (dt <= 0f)
            {
                return;
            }

            Vector3 delta = transform.position - lastPosition;
            delta.y = 0f;
            float instantSpeed = delta.magnitude / dt;
            lastPosition = transform.position;

            clientWalkSpeed = Mathf.Lerp(clientWalkSpeed, instantSpeed, 1f - Mathf.Exp(-clientAnimSmooth * dt));
            animator.SetFloat(walkingFloatParameter, clientWalkSpeed);
        }
    }
}
