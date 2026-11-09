using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Voyageur.Networking;

public class EnemyHealth : MonoBehaviour {

    public event Action onEnemyDied;
    public event Action onEnemyHurt;

    [Tooltip("The health pool of the enemy, when this reaches 0, he dies")]
    public int Health = 100;

    List<EnemyHitbox> enemyHitboxes = new List<EnemyHitbox>();
    NetworkEnemyAuthority networkEnemy;
    bool isDead;

    private void Awake() {
        networkEnemy = GetComponent<NetworkEnemyAuthority>();
    }

    private void Start() {
        enemyHitboxes = GetComponentsInChildren<EnemyHitbox>().ToList();
        enemyHitboxes.ForEach(enemyHitbox => enemyHitbox.onHit += onHit);
    }

    void onHit(EnemyBodyPartType enemyBodyPartType, bool isCritical, DamageSource source) {
        if (isDead || source == null) {
            return;
        }

        // Networked enemies: authority component owns damage validation / replication.
        if (networkEnemy != null && networkEnemy.IsSpawned) {
            networkEnemy.SubmitHit(source, isCritical);
            return;
        }

        ApplyHit(source.Damage, source.IsFromPlayer, isCritical, source);
    }

    /// <summary>
    /// Server-only entry used by <see cref="NetworkEnemyAuthority"/> after a validated hit.
    /// </summary>
    public void ApplyNetworkDamage(int damage, bool isFromPlayer, bool isCritical) {
        ApplyHit(damage, isFromPlayer, isCritical, source: null);
    }

    void ApplyHit(int damage, bool isFromPlayer, bool isCritical, DamageSource source) {
        if (isDead) {
            return;
        }

        if (isFromPlayer) {
            damage = PlayerStats.CalculatePlayerDamage(damage, isCritical);
        }

        if (source != null) {
            Events.invokeOnEnemyHit(gameObject, source);
        }

        Health -= damage;
        Debug.Log($"Ouch my health is {Health}");

        if (Health <= 0) {
            Health = 0;
            isDead = true;
            onEnemyDied?.Invoke();
        } else {
            onEnemyHurt?.Invoke();
        }
    }

}
