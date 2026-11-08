using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is a script that invokes an events when the player interacts with this objects, make sure to set the tag to interactable on this gameobject
/// </summary>
public class Interactable : MonoBehaviour {
    public UnityEvent onInteract;

    void Start() {
        Events.onPlayerInteracted += onPlayerInteracted;
    }

    private void onPlayerInteracted(GameObject interactable) {
        if (interactable.transform.IsChildOf(transform)) {
            onInteract?.Invoke();
        }
    }

    private void OnDestroy() {
        Events.onPlayerInteracted -= onPlayerInteracted;
    }
}
