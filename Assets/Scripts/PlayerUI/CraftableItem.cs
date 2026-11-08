using System;
using UnityEngine;
using UnityEngine.UI;

public class CraftableItem : MonoBehaviour {

    [Tooltip("The image component that renders the icon for the item")]
    public Image ItemImage;

    [Tooltip("The color to show on the item if the player can't craft it")]
    public Color CantCraftColor;

    [NonSerialized]
    public Item Item;

    Color initialImageColor;

    private void Start() {
        initialImageColor = ItemImage.color;
        render();
    }

    void render() {
        if (Item != null) {
            ItemImage.sprite = Item.Sprite;
            ItemImage.color = canCraft() ? initialImageColor : CantCraftColor;
        }
    }

    private void OnEnable() {
        render();
    }

    public void onPlayerClicked() {
        Craft();
    }

    bool canCraft() {
        var unmetRequirements = Item.CraftingRecipe.Find(requirement => {
            var requirementMet = PlayerItems.InventoryContains(requirement.ItemSubType, requirement.Amount);
            return !requirementMet;
        });

        return unmetRequirements == null;
    }

    void Craft() {
        if (canCraft()) {
            Events.invokeOnPlayerCraftedItem(Item);
        }
    }

}
