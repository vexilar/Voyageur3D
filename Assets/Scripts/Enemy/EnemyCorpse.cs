using UnityEngine;

public class EnemyCorpse : MonoBehaviour {

    EnemyHealth enemyHealth;
    new Collider collider;

    private void Start() {
        enemyHealth = GetComponentInParent<EnemyHealth>();
        enemyHealth.onEnemyDied += onEnemyDied;
        collider = GetComponent<Collider>();
    }

    void onEnemyDied() {
        collider.enabled = true;
    }

}
