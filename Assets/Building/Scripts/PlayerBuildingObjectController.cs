using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script is responsible for switching between placable objects in response to the players interaction with hotbar slots that contain placeable objects
/// This is a script is attached to the building placement object in the scene
/// When the player changes between placeable items in their hotbar, we enable different objects inside this object to show the player what he is about to place, and where
/// When one of its children (the placable preview object) is placed, it listens for this and invokes a global event to notify the game of the item having been placed
/// </summary>
public class PlayerBuildingObjectController : MonoBehaviour {

    Item selectedPlacableItem;

    List<ItemIdentifier> itemIdentifiers = new List<ItemIdentifier>();

    private void Start() {
        GetComponentsInChildren<PlayerPlacableObjectPreview>(true).ToList().ForEach(preview => preview.onPlayerPlacedObject += onPlayerPlacedObject);
        itemIdentifiers = GetComponentsInChildren<ItemIdentifier>(true).ToList();
    }

    private void Update() {
        Events.onHotBarSlotSelected += onHotbarSlotSelected;
        Events.onHotBarSlotDeselected += onHotBarSlotDeselected;
    }

    void onPlayerPlacedObject() {
        Events.invokeOnPlayerPlacedObject(selectedPlacableItem);
    }

    void onHotbarSlotSelected(Item item) {
        selectedPlacableItem = item;

        var foundIdentifier = itemIdentifiers.Find(itemIdentifier => itemIdentifier.ItemSubType == item.ItemSubType);

        if (foundIdentifier) {
            foundIdentifier.gameObject.SetActive(true);
        }
    }

    void onHotBarSlotDeselected(Item item) {
        var foundItem = itemIdentifiers.Find(itemIdentifier => itemIdentifier.ItemSubType == item.ItemSubType);

        if (foundItem) {
            foundItem.gameObject.SetActive(false);
        }
    }

}
