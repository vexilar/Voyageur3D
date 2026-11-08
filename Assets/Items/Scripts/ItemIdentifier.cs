using UnityEngine;

/// <summary>
/// This class is attached to anything in the game where you just need to identify something ambiguiously as a certain item
/// For example, it is attached to static objects in the scene that aren't true items that belong to the player
/// </summary>
public class ItemIdentifier : MonoBehaviour {

    [Tooltip("This uniquely identifies an item, use it whenever you are looking to associate items from on place to another")]
    public ItemSubType ItemSubType;

}
