using UnityEngine;

/// <summary>
/// This handles all loot based operations
/// </summary>
public static class LootAPI {

    /// <summary>
    /// This will acquire items randomly selected from the loot table
    /// </summary>
    /// <param name="lootTable">the table of descriptions to use in order to randomly select items</param>
    /// <param name="multiplier">an optional multiplier that is applied to the item amount</param>
    public static void GenerateLoot(LootTable lootTable, float multiplier = 1f) {
        var lootDescription = lootTable.LootDescriptions.GetRandom();

        if (ItemAPI.IsStackable(lootDescription.ItemSubType)) {
            var randomAmount = Random.Range(lootDescription.MinimumAmount, lootDescription.MaximumAmount + 1);
            randomAmount = (int)(randomAmount * multiplier);
            ItemAcquisition.AcquireItem(lootDescription.ItemSubType, randomAmount);
        } else {
            ItemAcquisition.AcquireItem(lootDescription.ItemSubType);
        }
    }

}