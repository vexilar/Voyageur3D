using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is attached to the ctafting item tooltip object, it renders item details in a tooltip
/// </summary>
public class CraftingItemTooltip : Tooltip {

    [Tooltip("The text component displaying the name of the item")]
    public TextMeshProUGUI ItemName;

    [Tooltip("The text component displaying the description of the item")]
    public TextMeshProUGUI ItemDescription;

    [Tooltip("The image component displaying the image of the item")]
    public Image ItemImage;

    [Tooltip("The parent that houses the craftable component items")]
    public GameObject ComponentContainer;

    [Tooltip("The prefab for a crafting component ui element")]
    public GameObject CraftingComponent;

    public override void Start() {
        base.Start();
        onTooltipOpened += render;
        onTooltipClosed += tooltipClosed;
    }

    private void render(TooltipTriggerInformation tooltipTriggerInformation) {
        ItemName.text = tooltipTriggerInformation.Item.Name;
        ItemDescription.text = tooltipTriggerInformation.Item.Description;
        ItemImage.sprite = tooltipTriggerInformation.Item.Sprite;

        tooltipTriggerInformation.Item.CraftingRecipe.ForEach(craftingRequirement => {
            var instance = Instantiate(CraftingComponent, ComponentContainer.transform);
            var craftingComponent = instance.GetComponent<CraftingComponent>();

            if (craftingComponent) {
                craftingComponent.CraftingRequirement = craftingRequirement;
            }
        });
    }

    void tooltipClosed(TooltipTriggerInformation tooltipTriggerInformation) {
        foreach(Transform child in ComponentContainer.transform) {
            Destroy(child.gameObject);
        }
    }

}
