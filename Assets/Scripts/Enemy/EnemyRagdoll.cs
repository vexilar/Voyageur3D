using UnityEngine;

public class EnemyRagdoll : MonoBehaviour {

    struct JointState {
        public CharacterJoint Joint;
        public Rigidbody ConnectedBody;
    }

    Animator animator;
    Rigidbody rootBody;
    EnemyHealth enemyHealth;
    JointState[] jointStates;
    Rigidbody[] boneBodies;

    void Awake() {
        animator = GetComponent<Animator>();
        rootBody = GetComponent<Rigidbody>();
        boneBodies = GetComponentsInChildren<Rigidbody>(true);

        var joints = GetComponentsInChildren<CharacterJoint>(true);
        jointStates = new JointState[joints.Length];
        for (var i = 0; i < joints.Length; i++) {
            jointStates[i] = new JointState {
                Joint = joints[i],
                ConnectedBody = joints[i].connectedBody
            };
        }

        SetRagdollActive(false);
    }

    void Start() {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.onEnemyDied += OnEnemyDied;
    }

    void OnEnemyDied() {
        if (animator) {
            animator.enabled = false;
        }

        if (rootBody) {
            rootBody.isKinematic = true;
        }

        SetRagdollActive(true);
    }

    void SetRagdollActive(bool active) {
        // CharacterJoints stay on the prefab for death ragdoll, but must be
        // disconnected while animated. Otherwise they fight the Animator /
        // NavMeshAgent and explode bone localPositions (stretchy mesh).
        foreach (var state in jointStates) {
            if (!state.Joint) {
                continue;
            }

            state.Joint.connectedBody = active ? state.ConnectedBody : null;
            state.Joint.enableCollision = active;
        }

        foreach (var body in boneBodies) {
            if (!body || body == rootBody) {
                continue;
            }

            body.interpolation = active ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
            body.collisionDetectionMode = active
                ? CollisionDetectionMode.ContinuousSpeculative
                : CollisionDetectionMode.Discrete;

            if (active) {
                body.isKinematic = false;
            } else {
                body.isKinematic = true;
            }
        }

        Debug.Log($"EnemyRagdoll on {name}: ragdoll={(active ? "on" : "off")}, joints={jointStates.Length}", this);
    }

}
