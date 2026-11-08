using System;
using System.Collections;
using UnityEngine;

// the enemy class is responsible for hearing and seeing the target
// it starts looking when the target is heard (currently a simple distance trigger)
// and it loses 'suspicion' after a set time of having not seen the target
// currently it only aggros the player (notice the .isPlayer check), but it is built to handle anything
// the idea behind enemy is that it doesnt have a dependency on other enemy classes (when possible)
public class Enemy : MonoBehaviour {

    public event Action onNoLongerSuspicious;
    public event Action<GameObject> onHeardTarget;
    public event Action<GameObject> onSeenTarget;

    [Range(-1f, 1f)]
    [Tooltip("How wide the enemy's cone of vision is, -1 means he has 360 vision, 0 means he sees anything in a 180 degree arc in front of him, and 1 would make him blind")]
    public float PeripheralVision = 0.2f;

    [Tooltip("How long the enemy will look for the target after hearing it")]
    public float SuspicionDuration = 10f;

    bool isTargetInRange;
    bool hasSeenTarget; // this is used to prevent the seenTarget event from firing more than once per interaction with the target
    bool isDead;

    EnemyHealth enemyHealth;
    GameObject target;
    Coroutine lookForTargetCoroutine;
    Coroutine suspicionDecayCoroutine;

    private void Start() {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.onEnemyDied += onEnemyDied;
    }

    void onEnemyDied() {
        isDead = true;
    }

    void targetSpotted() {
        onSeenTarget?.Invoke(target.gameObject);
        hasSeenTarget = true;

        if (suspicionDecayCoroutine != null) {
            StopCoroutine(suspicionDecayCoroutine);
        }

        suspicionDecayCoroutine = null;
    }

    bool canSeeTarget() {
        var directionToTarget = transform.position.direction(target.transform.position);
        return Vector3.Dot(directionToTarget, transform.forward) > PeripheralVision; // Is the enemy facing (somewhat) towards the target
    }

    private void OnTriggerExit(Collider other) {
        if (other.isPlayer() && !isDead) {
            target = null;
            isTargetInRange = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.isPlayer() && !isTargetInRange && !isDead) {
            target = other.gameObject;
            isTargetInRange = true;
            lookForTargetCoroutine = StartCoroutine(lookForTarget());

            if (!canSeeTarget()) {
                onHeardTarget?.Invoke(target);
                suspicionDecayCoroutine = StartCoroutine(suspicionDecay());
            }
        }
    }

    public void ResetAggro() {
        hasSeenTarget = false;
    }

    IEnumerator suspicionDecay() {
        yield return new WaitForSeconds(SuspicionDuration);
        loseSuspicion();
        suspicionDecayCoroutine = null;
    }

    void loseSuspicion() {
        onNoLongerSuspicious?.Invoke();
        hasSeenTarget = false;
    }

    IEnumerator lookForTarget() {
        while (isTargetInRange && !hasSeenTarget && !isDead) {
            if (canSeeTarget()) {
                targetSpotted();
            }

            yield return new WaitForSeconds(.3f);
        }

        lookForTargetCoroutine = null;
    }

}
