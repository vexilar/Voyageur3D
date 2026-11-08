using UnityEngine;

public class EnemyRagdoll : MonoBehaviour {

    Animator animator;
    Rigidbody rigid;
    EnemyHealth enemyHealth;

    void Start() {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.onEnemyDied += onEnemyDied;
    }

    void onEnemyDied() {
        if (animator) {
            animator.enabled = false;
        }

        if (rigid) {
            rigid.isKinematic = true;
        }
    }

}
