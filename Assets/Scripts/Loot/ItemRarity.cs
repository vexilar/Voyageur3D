
/// <summary>
/// This describes the level of rarity of an item, when loot is rolled, it will choose a table based on some loot roll logic, and look for any items 
/// that match that loot table assigned to the list of lootable items on that object, or a lower one in the case it can't find any of that rarity
/// </summary>
public enum ItemRarity {
    Empty, Common, Uncommon, Rare, ExceedinglyRare
}