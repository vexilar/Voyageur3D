using UnityEngine;

public class EnemyActivateRigidBodyOnDeath : MonoBehaviour { // this assumes rigid bodies are kinematic to start, when the enemy dies it brings the rigid body into physics simulation

    Rigidbody rigid;
    EnemyHealth enemyHealth;

    void Start() {
        rigid = GetComponent<Rigidbody>();
        enemyHealth = GetComponentInParent<EnemyHealth>();
        enemyHealth.onEnemyDied += onEnemyDied;
    }

    void onEnemyDied() {
        rigid.isKinematic = false;
    }

}
