using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShowCrossHatchOnHit : MonoBehaviour {

    [Tooltip("The target color of the crosshatch when active")]
    public Color HatchColor;

    [Range(0, 1)]
    [Tooltip("How quickly to hide the crosshatch")]
    public float FadeSpeed = .05f;

    Image image;
    Coroutine showCrossHatchCoroutine;

    private void Start() {
        image = GetComponent<Image>();
        Events.onEnemyHit += onEnemyHit;
    }

    void onEnemyHit(GameObject enemy, DamageSource damageSource) {
        if (damageSource.IsProjectile && damageSource.IsFromPlayer) {
            stopCrossHatchCoroutine();
            showCrossHatchCoroutine = StartCoroutine(showCrossHatch());
        }
    }

    void stopCrossHatchCoroutine() {
        if (showCrossHatchCoroutine != null) {
            StopCoroutine(showCrossHatchCoroutine);
        }

        showCrossHatchCoroutine = null;
    }

    IEnumerator showCrossHatch() {
        image.color = HatchColor;
        var retainAmount = 1 - FadeSpeed;

        while (image.color.a > 0) {
            image.color *= retainAmount;
            yield return new WaitForSeconds(.02f);
        }
    }

}
