using System;
using UnityEngine;

/// <summary>
/// This class represents any information about the target being hovered to relay to the tooltip ui to show
/// </summary>
[Serializable]
public class TooltipTriggerInformation {

    /// <summary>
    /// This is the rect transform of the ui component being hovered, used for calculating the position of the tooltip
    /// </summary>
    [NonSerialized]
    public RectTransform SenderRectTransform;

    /// <summary>
    /// This is the item that the tooltip trigger represents, it is optional, and only used when the thing being hovered represents an item in some way
    /// </summary>
    [NonSerialized]
    public Item Item;

    /// <summary>
    /// This is the type of tooltip this trigger relates to, tooltips use this value to know whether they should display or another tooltip should
    /// </summary>
    [Tooltip("This is the type of tooltip this trigger relates to, tooltips use this value to know whether they should display or another tooltip should")]
    public TooltipType TooltipType;
}
