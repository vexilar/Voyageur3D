
using System;
using System.Collections.Generic;

/// <summary>
/// The Item api gives you an accessible interface to deal with items in the game
/// </summary>
public static class ItemAPI {

    private static List<Item> items;

    public static void LoadItemsFromEditor(List<Item> itemDefinitions) {
        items = itemDefinitions;
    }

    /// <summary>
    /// This creates a new instance of an item for you
    /// </summary>
    /// <param name="itemSubType">The item type</param>
    /// <param name="amount">If the item is stackable, the amount of that item</param>
    /// <returns></returns>
    public static Item CreateItem(ItemSubType itemSubType, int amount = 0) {
        var foundItem = GetItem(itemSubType);

        var newAmount = foundItem.IsStackable ? amount : 0;

        var createdItem = new Item { // todo, find a way to clone the object, i tried to no avail
            Amount = newAmount,
            CraftingRecipe = foundItem.CraftingRecipe,
            Description = foundItem.Description,
            IsCraftable = foundItem.IsCraftable,
            IsStackable = foundItem.IsStackable,
            ItemSubType = foundItem.ItemSubType,
            ItemType = foundItem.ItemType,
            LootTable = foundItem.LootTable,
            Name = foundItem.Name,
            ResourceGatherTypes = foundItem.ResourceGatherTypes,
            ResourceGatheringEffectiveness = foundItem.ResourceGatheringEffectiveness,
            Sprite = foundItem.Sprite,
            ItemWorldPrefab = foundItem.ItemWorldPrefab
        };

        return createdItem;
    }

    /// <summary>
    /// Returns if the item is stackable
    /// </summary>
    /// <param name="itemSubType">The item type</param>
    /// <returns>Whether it is stackable</returns>
    public static bool IsStackable(ItemSubType itemSubType) {
        var foundItem = GetItem(itemSubType);
        return foundItem.IsStackable;
    }

    /// <summary>
    /// Gets a list of craftable items for a specified item type
    /// </summary>
    /// <param name="itemType">The item type to match by</param>
    /// <returns>A list of items matching the specified type that are craftable</returns>
    public static List<Item> GetCraftableItems(ItemType itemType) {
        var foundItems = GetItemsOfType(itemType);

        return foundItems.FindAll(item => item.IsCraftable);
    }

    /// <summary>
    /// Gets a list of items matching the specified type
    /// </summary>
    /// <param name="itemType">The item type</param>
    /// <returns>All items from the list that match the type</returns>
    public static List<Item> GetItemsOfType(ItemType itemType) {
        return items.FindAll(item => item.ItemType == itemType);
    }

    /// <summary>
    /// Get the item details using a type
    /// </summary>
    /// <param name="itemSubType">the type of item to find the details for</param>
    /// <returns>the item from the define collection of items</returns>
    public static Item GetItem(ItemSubType itemSubType) {
        var foundItem = items.Find(item => item.ItemSubType == itemSubType);

        if (foundItem == null) {
            throw new Exception($"It looks like {itemSubType} isn't defined inside the item defintions file attached to the item defintions PREFAB (dont change the one in the scene!)");
        }

        return foundItem;
    }
}