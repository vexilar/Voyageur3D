using System.Collections;
using UnityEngine;

public class PlayerFOVController : MonoBehaviour {

    [Tooltip("The amount of field of view to increase when actions that increase fov happen")]
    public float FieldOfViewIncrease = 10f;

    PlayerBow playerBow;
    new Camera camera;
    float initialFov;

    Coroutine lerpFieldOfViewCoroutine;

    private void Start() {
        camera = Camera.main;
        initialFov = camera.fieldOfView;

        playerBow = GetComponentInChildren<PlayerBow>(true);
        playerBow.onPlayerDrawBow += onPlayerDrawBow;
        playerBow.onPlayerRelease += onPlayerRelease;
    }

    void onPlayerDrawBow() {
        stopLerpFieldOfView();
        lerpFieldOfViewCoroutine = StartCoroutine(lerpFieldOfView(initialFov + FieldOfViewIncrease));
    }

    void stopLerpFieldOfView() {
        if (lerpFieldOfViewCoroutine != null) {
            StopCoroutine(lerpFieldOfViewCoroutine);
        }

        lerpFieldOfViewCoroutine = null;
    }

    IEnumerator lerpFieldOfView(float targetFov, float lerpSpeed = .1f) {
        while (camera.fieldOfView != targetFov) {
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFov, lerpSpeed);
            yield return new WaitForSeconds(.02f);
        }
    }

    void onPlayerRelease(float drawDuration, int bowDamage) {
        stopLerpFieldOfView();
        lerpFieldOfViewCoroutine = StartCoroutine(lerpFieldOfView(initialFov, .2f));
    }

}
