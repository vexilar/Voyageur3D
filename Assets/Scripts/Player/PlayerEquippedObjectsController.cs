using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class is attached to the player, it finds the items inside (which are objects that the player carries in front of him like an axe)
/// When relevant events are fired it manages the enabling and disabling of those objects
/// </summary>
public class PlayerEquippedObjectsController : MonoBehaviour {

    List<ItemIdentifier> itemIdentifiers = new List<ItemIdentifier>();

    private void Start() {
        itemIdentifiers = GetComponentsInChildren<ItemIdentifier>(true).ToList();
        Events.onHotBarSlotDeselected += onHotBarSlotDeselected;
        Events.onHotBarSlotSelected += onHotBarSlotSelected;
    }

    void onHotBarSlotSelected(Item item) {
        var foundIdentifier = itemIdentifiers.Find(identifier => identifier.ItemSubType == item.ItemSubType);

        if (foundIdentifier) {
            foundIdentifier.gameObject.SetActive(true);
        }
    }

    void onHotBarSlotDeselected(Item item) {
        var foundIdentifier = itemIdentifiers.Find(identifier => identifier.ItemSubType == item.ItemSubType);

        if (foundIdentifier) {
            foundIdentifier.gameObject.SetActive(false);
        }
    }

}
