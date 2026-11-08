using UnityEngine;

public class CursorController : MonoBehaviour {

    public Texture2D CursorPointer;
    public Texture2D CursorOpenHand;
    public Texture2D CursorClosedHand;

    private void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Events.onDraggableHovered += onDraggableHovered;
        Events.onDraggableUnHovered += onDraggableUnHovered;
        Events.onToggleMouseActive += onToggleMouseActive;
    }

    void onToggleMouseActive(bool isActive) {
        Cursor.visible = isActive;
        if (isActive) {
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void onDraggableUnHovered() {
        Cursor.SetCursor(CursorPointer, Vector2.zero, CursorMode.Auto);
    }

    private void onDraggableHovered() {
        Cursor.SetCursor(CursorOpenHand, Vector2.zero, CursorMode.Auto);
    }

}
