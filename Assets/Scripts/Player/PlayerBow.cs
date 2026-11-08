using System;
using System.Collections;
using UnityEngine;

public class PlayerBow : MonoBehaviour {

    public event Action onPlayerKnockArrow;
    public event Action onPlayerDrawBow;
    public event Action onCancelKnockArrow;
    public event Action onCancelDrawBow;
    public event Action<float, int> onPlayerRelease;

    [Tooltip("This is the damage applied to the arrow, so when it hits a target it will take the damage from the bow, plus the arrow damage, multiplied by the amount at which the player drew the bow")]
    public int Damage = 10;

    [Tooltip("The time it takes to knock an arrow before the bow begins to draw")]
    public float KnockArrowDuration = .3f;

    bool isBowDrawn;
    bool isArrowKnocked;
    float drawDuration;

    Coroutine knockArrowAndDrawBowCoroutine;

    private void OnDisable() {
        interupKnockAndDraw();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && !GameState.IsMouseActive) {
            knockArrowAndDrawBowCoroutine = StartCoroutine(knockArrowAndDrawBow());
        }

        if (Input.GetMouseButtonUp(0)) {
            if (isBowDrawn) {
                releaseString();
                stopKnockAndDrawCoroutine();
            } else {
                cancelDraw();
            }
        }

        if (isBowDrawn) {
            drawDuration += Time.deltaTime;
        }
    }

    void cancelDraw() {
        stopKnockAndDrawCoroutine();
    }

    void interupKnockAndDraw() {
        onCancelKnockArrow?.Invoke();
        onCancelDrawBow?.Invoke();
        stopKnockAndDrawCoroutine();
        isBowDrawn = false;
        isArrowKnocked = false;
        drawDuration = 0;
    }

    void stopKnockAndDrawCoroutine() {
        if (knockArrowAndDrawBowCoroutine != null) {
            StopCoroutine(knockArrowAndDrawBowCoroutine);
        }

        knockArrowAndDrawBowCoroutine = null;
    }

    void releaseString() {
        onPlayerRelease?.Invoke(drawDuration, Damage);
        isBowDrawn = false;
        isArrowKnocked = false;
        drawDuration = 0;
    }

    IEnumerator knockArrowAndDrawBow() {
        if (!isArrowKnocked) {
            onPlayerKnockArrow?.Invoke();
            isArrowKnocked = true;
            yield return new WaitForSeconds(KnockArrowDuration);
        }

        drawBow();
    }

    void drawBow() {
        isBowDrawn = true;
        onPlayerDrawBow?.Invoke();
        drawDuration = 0;
    }
}
