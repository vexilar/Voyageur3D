using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item is an actual item in the game, it has a unique id, and keeps track of that item throughout the game
/// REMINDER!!!! if you change the schema, make sure to update the Item API where it creates the item
/// </summary>
[Serializable]
public class Item {

    [Tooltip("The name of the item")]
    public string Name;

    [Tooltip("The sub type of the item")]
    public ItemSubType ItemSubType;

    [Tooltip("The base type of the item, used for many game functions")]
    public ItemType ItemType;

    [Tooltip("The sprite used for displaying the item in different UIs")]
    public Sprite Sprite;

    [Tooltip("The description of the item, shows up in certain places of the ui, tells the player more about the item")]
    public string Description;

    [Tooltip("If true, this indicates the item can be stacked, having a quantity associated to it")]
    public bool IsStackable;

    [Tooltip("If stackable, this represents the amount of this item being stacked")]
    public int Amount;

    [Tooltip("If true, the object can be crafted by the player")]
    public bool IsCraftable;

    [Tooltip("The types of resources this item can gather, must also be an equippable item")]
    public List<ResourceGatherType> ResourceGatherTypes = new List<ResourceGatherType>();

    [Tooltip("A multiplier that increases item gather amount")]
    public float ResourceGatheringEffectiveness = 1f;

    [Tooltip("If craftable is true, this defines which items are required to make it, and how many")]
    public List<CraftingRequirement> CraftingRecipe = new List<CraftingRequirement>();

    [Tooltip("The rarity of the item when used in the loot system, for example, if this is a rare gemstone, and placed in the loot pool when mining an 'iron rock', this defines how often " +
        "the player will receive this item when mining that iron ore object")]
    public ItemRarity LootTable;

    [Tooltip("This is the associated items prefab as it would exist in the game world")]
    public GameObject ItemWorldPrefab;

    [NonSerialized]
    public Guid UniqueId = Guid.NewGuid();
}