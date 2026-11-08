using UnityEngine;

/// <summary>
/// This script is attached to terrain, and uses the attached loot table script to generate items randomly on the terrain
/// </summary>
public class TerrainItemGenerator : MonoBehaviour {

    LootTable lootTable;
    Terrain terrain;

    void Start() {
        terrain = GetComponent<Terrain>();
        lootTable = GetComponent<LootTable>();
        createItems();
    }

    void createItems() {
        lootTable.LootDescriptions.ForEach(lootDescription => {
            var amountToGenerate = Random.Range(lootDescription.MinimumAmount, lootDescription.MaximumAmount + 1);

            for (int i = 0; i < amountToGenerate; i++) {
                var itemToInstantiate = ItemAPI.GetItem(lootDescription.ItemSubType).ItemWorldPrefab;
                var randomPosition = terrain.GetRandomPosition();

                Instantiate(itemToInstantiate, randomPosition, Quaternion.identity);
            }
        });

    }
}
