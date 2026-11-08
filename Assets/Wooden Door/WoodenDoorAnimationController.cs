using UnityEngine;

public class WoodenDoorAnimationController : MonoBehaviour {

    Animator animator;
    Door door;

    private void Start() {
        animator = GetComponent<Animator>();
        door = GetComponent<Door>();
        door.onDoorClosed += onDoorClosed;
        door.onDoorOpened += onDoorOpened;
    }

    void onDoorClosed() {
        animator.SetBool(WoodenDoorAnimationStates.Open, false);
    }

    void onDoorOpened() {
        animator.SetBool(WoodenDoorAnimationStates.Open, true);
    }

}

public static class WoodenDoorAnimationStates {
    public static string Open = "Open";
}
