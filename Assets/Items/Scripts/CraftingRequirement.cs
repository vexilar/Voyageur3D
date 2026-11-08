using System;
using UnityEngine;

/// <summary>
/// This describes the item type and amount of that item used in a crafting recipe, crafting recipe definition is found on the item class
/// </summary>
[Serializable]
public class CraftingRequirement {

    [Tooltip("The item type of a component used to make up a craftine recipe")]
    public ItemSubType ItemSubType;

    [Tooltip("The amount of this item needed as part of a crafting recipe")]
    public int Amount;

}