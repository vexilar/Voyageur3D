using System;
using TMPro;
using UnityEngine;

/// <summary>
/// This script is attached to the crafting group ui component, it takes in the craftable type, and renders the craftable items of that type
/// </summary>
public class CraftingGroup : MonoBehaviour {

    [Tooltip("The text component displaying the name of the crafting group")]
    public TextMeshProUGUI CraftingGroupName;

    [Tooltip("The container that parents the items to craft")]
    public GameObject CraftableItemsContainer;

    [Tooltip("The Item component that represents a craftable item")]
    public GameObject CraftableItem;

    [NonSerialized]
    public ItemType ItemType;

    private void Start() {
        Events.onPlayerCraftedItem += onPlayerCraftedItem;
    }

    private void OnEnable() {
        render();
    }

    private void onPlayerCraftedItem(Item itemCrafted) {
        render();
    }

    void render() {
        removeCraftableItems();

        CraftingGroupName.text = ItemType.ToString();

        var craftableItems = ItemAPI.GetCraftableItems(ItemType);

        craftableItems.ForEach(craftableItem => {
            var instance = Instantiate(CraftableItem, CraftableItemsContainer.transform);
            var craftableItemComponent = instance.GetComponent<CraftableItem>();
            var craftableItemTooltip = instance.GetComponent<TooltipTrigger>();
            craftableItemTooltip.TooltipTriggerInformation.Item = craftableItem;
            craftableItemComponent.Item = craftableItem;
        });
    }

    void removeCraftableItems() {
        foreach (Transform child in CraftableItemsContainer.transform) {
            Destroy(child.gameObject);
        }
    }

}
