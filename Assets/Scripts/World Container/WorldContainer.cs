using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is attached to containers the player opens, and keeps track of what is in the container, as well as raises an event when the player
/// interacts with this object
/// </summary>
public class WorldContainer : MonoBehaviour {

    [Tooltip("The name of the container, this shows in the player ui")]
    public string Name;

    [Tooltip("The items inside the container")]
    public List<Item> Items = new List<Item>();

    private void Start() {
        Events.onPlayerInteracted += onPlayerInteracted;
        Events.onWorldContainerUpdated += onWorldContainerUpdated;
    }

    void onWorldContainerUpdated(WorldContainer worldContainer, List<Item> items) {
        if (worldContainer == this) {
            Items = items;
        }
    }

    void onPlayerInteracted(GameObject interactable) {
        if (interactable == gameObject || interactable.transform.IsChildOf(transform)) {
            Events.invokeOnPlayerOpenedContainer(this);
        }
    }

}
