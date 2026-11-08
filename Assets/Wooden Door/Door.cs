using System;
using UnityEngine;

public class Door : MonoBehaviour {

    public event Action onDoorOpened;
    public event Action onDoorClosed;

    bool isOpen;

    private void Start() {
        Events.onPlayerInteracted += onPlayerInteracted;
    }

    void onPlayerInteracted(GameObject interactable) {
        if (interactable == gameObject || interactable.transform.IsChildOf(transform)) {
            interactWithDoor();
        }
    }

    void interactWithDoor() {
        isOpen = !isOpen;

        if (isOpen) {
            onDoorOpened?.Invoke();
        } else {
            onDoorClosed?.Invoke();
        }
    }

}
