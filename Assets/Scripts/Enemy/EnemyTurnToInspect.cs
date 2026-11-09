using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTurnToInspect : MonoBehaviour {

    [Tooltip("The delay between when the enemy hears something, and turns to inspect it")]
    public float TurnToInspectDelay = 5f;

    public event Action onTurnAroundToInspect;

    bool isDead;

    NavMeshAgent navMeshAgent;
    Enemy enemy;
    EnemyPursue enemyPursue;
    EnemyHealth enemyHealth;

    Coroutine turnToInspectCoroutine;

    private void Start() {
        enemy = GetComponent<Enemy>();
        enemyPursue = GetComponent<EnemyPursue>();
        enemyHealth = GetComponent<EnemyHealth>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        enemy.onHeardTarget += onHeardTarget;
        enemy.onSeenTarget += onSeenTarget;
        enemy.onNoLongerSuspicious += onLostTarget;
        enemyPursue.onTargetAbandoned += onLostTarget;
        enemyHealth.onEnemyDied += onEnemyDied;
    }

    void onEnemyDied() {
        isDead = true;
        interuptTurnToInspect();
    }

    void onLostTarget() {
        interuptTurnToInspect();
    }

    void onHeardTarget(GameObject target) {
        turnToInspectCoroutine = StartCoroutine(turnToInspect());
    }

    void onSeenTarget(GameObject target) {
        interuptTurnToInspect();
    }

    void interuptTurnToInspect() {
        navMeshAgent.TryResetPath();

        if (turnToInspectCoroutine != null) {
            StopCoroutine(turnToInspectCoroutine);
        }

        turnToInspectCoroutine = null;
    }

    IEnumerator turnToInspect() {
        yield return new WaitForSeconds(TurnToInspectDelay);

        var destination = transform.position + ((transform.forward * -1) * 10);
        NavMesh.SamplePosition(destination, out var meshDestination, 99, NavMesh.AllAreas);

        if (meshDestination.hit) {
            navMeshAgent.TrySetDestination(meshDestination.position);
        }

        onTurnAroundToInspect?.Invoke();

        turnToInspectCoroutine = null;
    }

}