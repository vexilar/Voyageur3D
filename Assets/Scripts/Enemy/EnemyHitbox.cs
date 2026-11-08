using System;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour {

    public event Action<EnemyBodyPartType, bool, DamageSource> onHit;

    [Tooltip("The body part type associated to this enemy collider, typically used to figure out damage amounts or play specific animations")]
    public EnemyBodyPartType enemyBodyPartType;

    [Tooltip("If true, this spot is a critical area for the enemy, causing the player's critical hit multiplier to affect this body part")]
    public bool isCritical;

    EnemyHealth enemyHealth;
    bool isDead;

    private void Start() {
        enemyHealth = GetComponentInParent<EnemyHealth>();
        enemyHealth.onEnemyDied += onEnemyDied;
    }

    void onEnemyDied() {
        isDead = true;
    }

    private void OnTriggerEnter(Collider other) {
        enemyHit(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        enemyHit(collision.gameObject);
    }

    void enemyHit(GameObject sourceObject) {
        if (!isDead) {
            var source = sourceObject.GetComponent<DamageSource>();

            if (source && !source.ShouldBeIgnored) {
                source.onHitCallBack();
                onHit?.Invoke(enemyBodyPartType, isCritical, source);
            }
        }
    }
}

public enum EnemyBodyPartType {
    LeftFoot, LeftShin, LeftThigh, RightFoot, RightShin, RightThigh, Body, RightArm, RightForarm, RightHand, LeftArm, Leftforarm, LeftHand, Head
}
