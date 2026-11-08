using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This exposes the items the player possesses in a global way
/// </summary>
public static class PlayerItems {

    /// <summary>
    /// The items in the player's inventory
    /// </summary>
    public static List<Item> InventoryItems = new List<Item>();

    /// <summary>
    /// Evaluates if the inventory contains an item, and the amount of that item
    /// </summary>
    /// <param name="itemSubType">The item type to check for in the inventory</param>
    /// <param name="amount">The amount of that item to check for, optional, if left unused, it will not evaluate the amount</param>
    /// <returns>A boolean representing if that items exists in the inventory</returns>
    public static bool InventoryContains(ItemSubType itemSubType, int amount = 0) {
        var foundItems = InventoryItems.FindAll(item => item.ItemSubType == itemSubType);

        if (amount > 0) {
            var amountOfItem = foundItems.Sum(item => item.Amount);
            return amountOfItem >= amount;
        } else {
            return foundItems.Count > 0;
        }
    }

}
