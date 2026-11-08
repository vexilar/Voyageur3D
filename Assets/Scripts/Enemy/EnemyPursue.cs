using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPursue : MonoBehaviour {

    [Tooltip("A delay between when the enemy sees it's target, and starts chasing it, helpful for when you want to play an aggro animation")]
    public float DelayPursuit = 2.5f;

    [Tooltip("The speed at which the enemy moves to pursue it's target")]
    public float PursuitSpeed = 8f;

    [Tooltip("How close to the target should it pursue")]
    public float TargetDistance = 10f;

    [Tooltip("The distance the enemy will travel during pursuit of a target, before returning to within his patrol radius")]
    public float LeashRadius = 75f;

    [Tooltip("The duration in which to pause pursuit upon reaching the pursuit target, helpful for letting attack animations finish playing")]
    public float DelayAfterReachingDestination = 2f;

    bool isInsideLeashArea => Vector3.Distance(transform.position, initialPosition) < LeashRadius;

    public event Action onReachedTarget;
    public event Action onChaseAfterReached;
    public event Action onPursue;
    public event Action onTargetAbandoned;

    Vector3 initialPosition;
    Enemy enemy;
    EnemyHealth enemyHealth;
    NavMeshAgent navMeshAgent;
    Coroutine chaseTargetCoroutine;
    float initialSpeed;
    bool isDead;

    private void Start() {
        enemy = GetComponent<Enemy>();
        enemyHealth = GetComponent<EnemyHealth>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        initialPosition = transform.position;
        initialSpeed = navMeshAgent.speed;

        enemy.onSeenTarget += onSeenTarget;
        enemyHealth.onEnemyDied += onEnemyDied;
    }

    void onEnemyDied() {
        isDead = true;
        stopPursuing();
    }

    void onSeenTarget(GameObject target) {
        chaseTargetCoroutine = StartCoroutine(chaseTarget(target));
    }

    IEnumerator chaseTarget(GameObject target) {
        yield return new WaitForSeconds(DelayPursuit);
        navMeshAgent.speed = PursuitSpeed;

        bool reachedTarget = false;
        bool hasPursuedPreviously = false;
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        while (isInsideLeashArea && !isDead) {
            if (distanceToTarget > TargetDistance) { // pursue the target
                if (reachedTarget) {
                    onChaseAfterReached?.Invoke();
                    onPursue?.Invoke();
                }

                if (!hasPursuedPreviously) {
                    hasPursuedPreviously = true;
                    onPursue?.Invoke();
                }

                NavMesh.SamplePosition(target.transform.position, out var targetPosition, 99, NavMesh.AllAreas);

                if (targetPosition.hit) {
                    navMeshAgent.SetDestination(targetPosition.position);
                }

                reachedTarget = false;
            } else {
                if (!reachedTarget) { // target reached
                    navMeshAgent.ResetPath();
                    onReachedTarget?.Invoke();
                    reachedTarget = true;
                    yield return new WaitForSeconds(DelayAfterReachingDestination);
                }
            }

            distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            yield return new WaitForSeconds(.3f);
        }

        if (!isDead) {
            abandonTarget();
        }
    }

    void stopPursuing() {
        enemy.ResetAggro();
        navMeshAgent.ResetPath();

        if (chaseTargetCoroutine != null) {
            StopCoroutine(chaseTargetCoroutine);
        }

        chaseTargetCoroutine = null;

        navMeshAgent.speed = initialSpeed;
    }

    void abandonTarget() {
        enemy.ResetAggro();
        stopPursuing();
    }
}
