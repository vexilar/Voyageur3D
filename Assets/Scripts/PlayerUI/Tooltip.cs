using System;
using UnityEngine;

/// <summary>
/// This script is attached to a tooltip ui object, it toggles a tooltip and handles the placement of it depending on where the hovered trigger is onscreen
/// </summary>
public class Tooltip : MonoBehaviour {

    [Tooltip("The Togglable component to show and hide the item tooltip")]
    public GameObject Toggleable;

    [Tooltip("The type of tooltip, this corresponds to the tooltip to open when a trigger is hovered")]
    public TooltipType TooltipType;

    public event Action<TooltipTriggerInformation> onTooltipOpened;
    public event Action<TooltipTriggerInformation> onTooltipClosed;

    RectTransform rectTransform;
    Vector3 offsetLeft;
    Vector3 offsetRight;
    Vector3 size;

    public virtual void Start() {
        rectTransform = GetComponent<RectTransform>();
        size = rectTransform.sizeDelta * rectTransform.localScale;

        offsetLeft = new Vector3(-size.x / 2, 0, 0);
        offsetRight = new Vector3(size.x / 2, 0, 0);

        Events.onShowTooltip += onShowTooltip;
        Events.onHideTooltip += onHideTooltip;
    }

    private void onHideTooltip(TooltipTriggerInformation tooltipTriggerInformation) {
        if (tooltipTriggerInformation.TooltipType == TooltipType) {
            onTooltipClosed?.Invoke(tooltipTriggerInformation);
            Toggleable.SetActive(false);
        }
    }

    private void onShowTooltip(TooltipTriggerInformation tooltipTriggerInformation) {
        if (tooltipTriggerInformation.TooltipType == TooltipType) {
            Toggleable.SetActive(true);

            var isCloserToRight = Mathf.Abs(tooltipTriggerInformation.SenderRectTransform.position.x - Screen.width) < Screen.width / 2;
            var offset = isCloserToRight ? offsetLeft : offsetRight;

            transform.position = tooltipTriggerInformation.SenderRectTransform.position + offset;

            onTooltipOpened?.Invoke(tooltipTriggerInformation);
        }
    }

}
