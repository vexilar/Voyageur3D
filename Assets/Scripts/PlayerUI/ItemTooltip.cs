using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is attached to the item tooltip object, it renders item details in a tooltip
/// </summary>
public class ItemTooltip : Tooltip {

    [Tooltip("The text component displaying the name of the item")]
    public TextMeshProUGUI ItemName;

    [Tooltip("The text component displaying the description of the item")]
    public TextMeshProUGUI ItemDescription;

    [Tooltip("The image component displaying the image of the item")]
    public Image ItemImage;

    public override void Start() {
        base.Start();
        onTooltipOpened += render;
    }

    private void render(TooltipTriggerInformation tooltipTriggerInformation) {
        ItemName.text = tooltipTriggerInformation.Item.Name;
        ItemDescription.text = tooltipTriggerInformation.Item.Description;
        ItemImage.sprite = tooltipTriggerInformation.Item.Sprite;
    }

}
