using System;
using System.Collections;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour {

    public event Action onMeleeAttack;

    [Tooltip("The time between starting attacks")]
    public float TimeBetweenAttacks = 3f;

    bool isDead;

    EnemyPursue enemyPursue;
    Enemy enemy;
    EnemyHealth enemyHealth;

    Coroutine attackCoroutine;

    private void Start() {
        enemy = GetComponent<Enemy>();
        enemyPursue = GetComponent<EnemyPursue>();
        enemyHealth = GetComponent<EnemyHealth>();

        enemy.onNoLongerSuspicious += stopAttacking;
        enemyPursue.onChaseAfterReached += stopAttacking;
        enemyPursue.onTargetAbandoned += stopAttacking;
        enemyPursue.onReachedTarget += onReachedTarget;
        enemyHealth.onEnemyDied += onEnemyDied;
    }

    void onEnemyDied() {
        isDead = true;
        stopAttacking();
    }

    void stopAttacking() {
        if (attackCoroutine != null) {
            StopCoroutine(attackCoroutine);
        }

        attackCoroutine = null;
    }

    void onReachedTarget() {
        attackCoroutine = StartCoroutine(attack());
    }

    IEnumerator attack() {
        while (!isDead) {
            onMeleeAttack?.Invoke();
            yield return new WaitForSeconds(TimeBetweenAttacks);
        }
    }

}
