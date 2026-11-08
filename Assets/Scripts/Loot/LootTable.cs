using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is attached to anything that the player ends up looting, corpses, containers, trees, etc
/// </summary>
public class LootTable : MonoBehaviour {

    /// <summary>
    /// The list of items and additional information, this is what the loot api will pull from
    /// </summary>
    public List<LootDescription> LootDescriptions = new List<LootDescription>();

}
