using UnityEngine;

/// <summary>
/// This is an item source that hooks up to the player interact, when a player interacts with this object it will distribute loot defined in the loot table
/// </summary>
public class ItemSource : MonoBehaviour {

    [Tooltip("The amount of uses before destroying the item source (min value used in randomizer)")]
    public int DestroyAfterUsesMin = 3;

    [Tooltip("The amount of uses before destroying the item source (max value used in randomizer)")]
    public int DestroyAfterUsesMax = 5;

    [Tooltip("Should the object be destroyed after it's been used a certain amount of times")]
    public bool ShouldDestroy = true;

    LootTable lootTable;
    int timesUsed;
    int destroyAfterUses;

    private void Start() {
        lootTable = GetComponent<LootTable>();
        destroyAfterUses = Random.Range(DestroyAfterUsesMin, DestroyAfterUsesMax + 1);
    }

    /// <summary>
    /// This method is usually invoked from the editor on the player interact script (assigned in editor)
    /// </summary>
    public void onPlayerInteracted() {
        LootAPI.GenerateLoot(lootTable);
        timesUsed++;

        if (timesUsed >= destroyAfterUses && ShouldDestroy) {
            Destroy(gameObject);
        }
    }

}
