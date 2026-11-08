using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWander : MonoBehaviour {
    // Currently we wander right away, then stop wandering if the player is heard, and continue wandering when he leaves the trigger area

    [Tooltip("The circle the enemy patrols within")]
    public float WanderRadius = 15f;

    [Tooltip("Duration between patrols")]
    public float WaitDuration = 5f;

    bool isDead;

    Enemy enemy;
    EnemyPursue enemyPursue;
    EnemyHealth enemyHealth;
    NavMeshAgent navMeshAgent;
    Vector3 initialPosition;

    Coroutine runningWanderRoutine;

    void Start() {
        enemy = GetComponent<Enemy>();
        enemyPursue = GetComponent<EnemyPursue>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();

        enemy.onHeardTarget += onHeardTarget;
        enemy.onSeenTarget += onSeenTarget;
        enemy.onNoLongerSuspicious += startWandering;
        enemyPursue.onTargetAbandoned += startWandering;
        enemyHealth.onEnemyDied += onEnemyDied;

        initialPosition = transform.position;
        runningWanderRoutine = StartCoroutine(wander());
    }

    void onEnemyDied() {
        isDead = true;
        stopWandering();
    }

    void onHeardTarget(GameObject target) {
        stopWandering();
    }

    void onSeenTarget(GameObject target) {
        stopWandering();
    }

    void stopWandering() {
        navMeshAgent.ResetPath();
        if (runningWanderRoutine != null) {
            StopCoroutine(runningWanderRoutine);
        }

        runningWanderRoutine = null;
    }

    void startWandering() {
        runningWanderRoutine = StartCoroutine(wander());
    }

    IEnumerator wander() {
        while (!isDead) {
            yield return new WaitForSeconds(WaitDuration);

            // pick a spot within a sphere around the enemy's starting position
            var randomPosition = (Random.insideUnitSphere * WanderRadius) + initialPosition;
            NavMesh.SamplePosition(randomPosition, out var wanderPosition, WanderRadius, NavMesh.AllAreas);

            if (wanderPosition.hit) {
                navMeshAgent.SetDestination(wanderPosition.position);
            }

            bool hasReachedPosition = false;
            float wanderTimout = 0f;

            while (!hasReachedPosition) {
                yield return new WaitForSeconds(.5f);
                wanderTimout += .5f;

                // stop waiting once we've reached the position we intended to get to, or 20 seconds passed
                hasReachedPosition = Vector3.Distance(transform.position, wanderPosition.position) < .5f || wanderTimout >= 20f;
            }
        }
    }

}
