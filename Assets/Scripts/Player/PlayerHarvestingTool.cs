using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This script is attached to harvesting tools equipped by the player inside the fps controller
/// </summary>
public class PlayerHarvestingTool : MonoBehaviour {

    [Tooltip("The direction the player is looking")]
    public Transform PlayerLook;

    [Tooltip("The duration it takes to swing, used to delay the gather logic between when the player swing and hits")]
    public float SwingDuration = .167f;

    [Tooltip("The duration it takes to recover from hitting a resource")]
    public float HitRecoveryDuration = .75f;

    [Tooltip("The duration it takes to recover from missing a resource")]
    public float MissRecoveryDuration = 1f;

    public event Action onSwung;
    public event Action onHit;
    public event Action onMiss;

    Coroutine swingCoroutine;

    private void Update() {
        if (Input.GetMouseButtonDown(0) && !GameState.IsMouseActive) {
            swing();
        }
    }

    void swing() {
        if (swingCoroutine == null) {
            swingCoroutine = StartCoroutine(swingAndWait());
        }
    }

    void interuptSwingCoroutine() {
        if (swingCoroutine != null) {
            StopCoroutine(swingCoroutine);
        }

        swingCoroutine = null;
    }

    IEnumerator swingAndWait() {
        onSwung?.Invoke();
        yield return new WaitForSeconds(SwingDuration);

        var resourceHit = raycastForResource(out var resourceRaycastHit);
        var treeHit = raycastForTree(out var treeRaycastHit);

        if (treeHit) {
            Events.invokeOnAxeHitTree(treeRaycastHit);
        }

        if (resourceHit || treeHit) {
            onHit?.Invoke();
            yield return new WaitForSeconds(HitRecoveryDuration);
        } else {
            onMiss?.Invoke();
            yield return new WaitForSeconds(MissRecoveryDuration);
        }

        swingCoroutine = null;
    }

    bool raycastForResource(out RaycastHit raycastHit) {
        var raycastRequest = new RaycastRequest {
            Origin = PlayerLook.position,
            Direction = PlayerLook.forward,
            Distance = 3f,
            LayerMask = LayerMasks.Default
        };

        return HarvestAPI.HarvestResource(raycastRequest, out raycastHit);
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
        interuptSwingCoroutine();
    }

}
