using UnityEngine;

public class ClampToTarget : MonoBehaviour {

    [Tooltip("This is the target object you would like to clamp to")]
    public GameObject Target;

    [Tooltip("The offset to maintain from the target object")]
    public Vector3 Offset;

    [Tooltip("Whether to put the object in front of the object, helpful for putting a weapon in front of a player")]
    public bool ClampInFrontOfTarget;

    [Tooltip("How far to put the object in front of the target, only applicable if ClampInFrontOfTarget is true")]
    public float DistanceInFrontOfTarget = 1f;

    [Tooltip("If this is true, the object with match the target objects rotation")]
    public bool MatchTargetRotation;

    private void Update() {
        var destination = Target.transform.position + Offset;

        if (ClampInFrontOfTarget) {
            destination += Target.transform.forward * DistanceInFrontOfTarget;
        }

        if (MatchTargetRotation) {
            transform.forward = Target.transform.forward;
        }

        transform.position = destination;
    }

}
