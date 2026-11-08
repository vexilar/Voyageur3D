using System;
using UnityEngine;

/// <summary>
/// This describes an entry in the loot table
/// </summary>
[Serializable]
public class LootDescription {

    [Tooltip("This is the item type")]
    public ItemSubType ItemSubType;

    [Tooltip("If stackable, this is the minimum amount of this item that will appear when selected")]
    public int MinimumAmount;

    [Tooltip("If stackable, this is the maximum amount of this item that will appear when selected")]
    public int MaximumAmount;
}
