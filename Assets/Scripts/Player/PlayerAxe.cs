using System;
using System.Collections;
using UnityEngine;

public class PlayerAxe : MonoBehaviour {

    [Tooltip("The direction the player is looking")]
    public Transform PlayerLook;

    public event Action onAxeSwung;
    public event Action onAxeHit;
    public event Action onAxeMiss;

    Coroutine swingAxeCoroutine;

    private void Update() {
        if (Input.GetMouseButtonDown(0) && !GameState.IsMouseActive) {
            swingAxe();
        }
    }

    void swingAxe() {
        if (swingAxeCoroutine == null) {
            swingAxeCoroutine = StartCoroutine(swingAxeAndWait());
        }
    }

    void interuptSwingAxeCoroutine() {
        if (swingAxeCoroutine != null) {
            StopCoroutine(swingAxeCoroutine);
        }

        swingAxeCoroutine = null;
    }

    IEnumerator swingAxeAndWait() {
        onAxeSwung?.Invoke();
        yield return new WaitForSeconds(.167f);

        if (raycastForTree(out var raycastHit)) {
            Events.invokeOnAxeHitTree(raycastHit);
            onAxeHit?.Invoke();
            yield return new WaitForSeconds(.25f);
        } else {
            onAxeMiss?.Invoke();
            yield return new WaitForSeconds(.65f);
        }

        swingAxeCoroutine = null;
    }

    bool raycastForTree(out RaycastHit raycastHit) {
        var raycastRequest = new RaycastRequest {
            Origin = PlayerLook.position,
            Direction = PlayerLook.forward,
            Distance = 3f,
            LayerMask = LayerMasks.Default
        };
       
        return TerrainAPI.RaycastForTree(raycastRequest, out raycastHit);
    }

    private void OnDisable() {
        interuptSwingAxeCoroutine();
    }

}
