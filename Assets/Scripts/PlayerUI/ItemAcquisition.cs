using UnityEngine;

/// <summary>
/// This script is called when an item is programatically added to the player's inventory
/// </summary>
public static class ItemAcquisition {

    /// <summary>
    /// This generates items and puts them in the player's inventory
    /// </summary>
    /// <param name="itemSubType">the type of item</param>
    /// <param name="amount">the amount of item (optional)</param>
    public static void AcquireItem(ItemSubType itemSubType, int amount = 0) {
        var newItem = ItemAPI.CreateItem(itemSubType, amount);

        var displayAmount = amount > 1 ? amount.ToString() : " a ";
        var pluraltext = amount > 1 ? "s" : "";
        SystemTextAPI.AddText($"You acquired {displayAmount} {newItem.Name}{pluraltext}");

        Events.invokeOnItemAcquired(newItem);
    }

}