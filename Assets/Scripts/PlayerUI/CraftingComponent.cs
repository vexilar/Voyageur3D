using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingComponent : MonoBehaviour {

    [Tooltip("The image component for displaying the item image")]
    public Image ItemImage;

    [Tooltip("The text component to display the amount of the item required")]
    public TextMeshProUGUI ItemAmount;

    [NonSerialized]
    public CraftingRequirement CraftingRequirement;

    [Tooltip("The color to set the item when the player doesn't have the required component amount, indicating they need more")]
    public Color DisabledImageColor;

    Item item;
    Color initialImageColor;

    private void Start() {
        initialImageColor = ItemImage.color;
        item = ItemAPI.GetItem(CraftingRequirement.ItemSubType);
        render();
    }

    void render() {
        ItemImage.sprite = item.Sprite;
        ItemAmount.text = CraftingRequirement.Amount.ToString();

        var requirementMet = PlayerItems.InventoryContains(CraftingRequirement.ItemSubType, CraftingRequirement.Amount);
        var color = requirementMet ? initialImageColor : DisabledImageColor;
        ItemImage.color = color;
        ItemAmount.color = color;
    }

}
