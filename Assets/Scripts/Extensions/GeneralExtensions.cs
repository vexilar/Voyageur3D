using UnityEngine;

public static class GeneralExtensions {

    /// <summary>
    /// This returns true if the collider's gameobject is the player(fps controller)
    /// </summary>
    public static bool isPlayer(this Collider collider) {
        return collider.gameObject.name == "FPSController";
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