using UnityEngine;

public static class GeneralExtensions {

    /// <summary>
    /// True for the single-player FPSController or a networked player avatar.
    /// DetectPlayer triggers only collide with the Player physics layer, so avatars
    /// should also live on that layer — this still accepts tag/name fallbacks.
    /// </summary>
    public static bool isPlayer(this Collider collider) {
        if (collider == null) {
            return false;
        }

        var go = collider.gameObject;
        if (go.CompareTag("Player") || go.layer == LayerMask.NameToLayer("Player")) {
            return true;
        }

        if (go.name == "FPSController") {
            return true;
        }

        var root = go.transform.root != null ? go.transform.root.gameObject : go;
        if (root.CompareTag("Player") || root.layer == LayerMask.NameToLayer("Player")) {
            return true;
        }

        return root.name.StartsWith("NetworkPlayer");
    }

    /// <summary>
    /// This returns the direction from point A to B
    /// </summary>
    public static Vector3 direction(this Vector3 positionA, Vector3 positionB) {
        var heading = positionB - positionA;
        var distance = heading.magnitude;
        var direction = heading / distance;
        return direction;
    }

}