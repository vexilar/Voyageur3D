using System;
using UnityEngine;

public class HarvestableResource : MonoBehaviour {

    public event Action onResourceHit;

    [Tooltip("The minimum amount of hits this resource spawns with")]
    public int MinimumHealth = 3;

    [Tooltip("The maximum amount of hits this resource spawns with")]
    public int MaximumHealth = 5;

    [Tooltip("The resource type assigned to this resource")]
    public ResourceGatherType ResourceGatherType;

    LootTable lootTable;
    int health;
    Item equippedItem;

    private void Start() {
        Events.onHotBarSlotSelected += onHotbarSlotSelected;
        lootTable = GetComponent<LootTable>();
        health = UnityEngine.Random.Range(MinimumHealth, MaximumHealth + 1);
    }

    private void onHotbarSlotSelected(Item item) {
        equippedItem = item;
    }

    public bool ResourceHit() {
        if (equippedItem != null && equippedItem.ResourceGatherTypes.Contains(ResourceGatherType)) {
            onResourceHit?.Invoke();

            health--;

            LootAPI.GenerateLoot(lootTable, equippedItem.ResourceGatheringEffectiveness);

            if (health <= 0) {
                Destroy(gameObject);
            }

            return true;
        }

        return false;
    }

}
