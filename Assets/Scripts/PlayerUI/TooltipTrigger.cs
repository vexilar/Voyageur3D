using System;
using UnityEngine;

/// <summary>
/// This script triggers the tooltip to show information
/// </summary>
public class TooltipTrigger : MonoBehaviour {

    [Tooltip("Information about the tooltip trigger")]
    public TooltipTriggerInformation TooltipTriggerInformation = new TooltipTriggerInformation();

    bool isActive;

    private void Start() {
        TooltipTriggerInformation.SenderRectTransform = GetComponent<RectTransform>();
    }

    public void ShowTooltip() {
        isActive = true;
        Events.invokeOnShowTooltip(TooltipTriggerInformation);
    }

    public void HideTooltip() {
        isActive = false;
        Events.invokeOnHideTooltip(TooltipTriggerInformation);
    }

    private void OnDisable() {
        if (isActive) {
            Events.invokeOnHideTooltip(TooltipTriggerInformation);
            isActive = false;
        }
    }

}
