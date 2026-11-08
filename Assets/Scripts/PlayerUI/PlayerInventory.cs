using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

/// <summary>
/// This is attached to the player inventory, it communicates with the draggable item container class which is a child to handle item information
/// It shows and hides the inventory ui, it also handles the response to when a player crafts an item
/// </summary>
public class PlayerInventory : MonoBehaviour {

    [Tooltip("The container object to toggle when showing and hiding the inventory")]
    public GameObject ContainerToToggle;

    DraggableItemsContainer draggableItemsContainer;

    ObservableCollection<Item> Items => draggableItemsContainer.Items;

    private void Start() {
        Events.onItemAcquired += onItemAcquired;
        Events.onPlayerToggleInventory += onToggleInventory;
        Events.onPlayerCraftedItem += onPlayerCraftedItem;

        draggableItemsContainer = GetComponent<DraggableItemsContainer>();
        draggableItemsContainer.onItemsChanged += onItemsChanged;
    }

    private void onPlayerCraftedItem(Item item) {
        ItemAcquisition.AcquireItem(item.ItemSubType, 1);

        item.CraftingRecipe.ForEach(component => {
            consumeItem(component.ItemSubType, component.Amount);
        });
    }

    void consumeItem(ItemSubType itemSubType, int amount = 0) {
        var foundItem = Items.ToList().Find(item => itemSubType == item.ItemSubType);

        if (foundItem != null) {
            var foundIndex = Items.IndexOf(foundItem);

            if (amount == 0) { // no amount specified, remove the entire item
                Items.RemoveAt(foundIndex);
            } else {
                foundItem.Amount -= amount;

                if (foundItem.Amount <= 0) { // stack reduced to 0, remove the entire item
                    Items.RemoveAt(foundIndex);
                } else {
                    Items[foundIndex] = foundItem; // item amount reduced, but still has some left, just reduce the amount
                }
            }
        }
    }

    private void onToggleInventory(bool isOpen) {
        GameState.IsPlayerInventoryOpen = isOpen;
        ContainerToToggle.SetActive(isOpen);
    }

    private void onItemAcquired(Item item) {
        // stack the items if possible
        if (item.IsStackable) {
            var foundItem = Items.ToList().Find(i => i.ItemSubType == item.ItemSubType);
            if (foundItem != null) {
                foundItem.Amount += item.Amount;
                Items[Items.IndexOf(foundItem)] = foundItem;
                return;
            }
        }

        Items.Add(item);
    }

    void onItemsChanged(List<Item> items) {
        PlayerItems.InventoryItems = items;
    }

}
