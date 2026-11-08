using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this script specifically handles managing the health of trees, their resource distribution, and when they are no longer harvestable 
/// once a tree is hit, we track the tree and it's health in a list, it is attached to the tree harvest controller game object
/// </summary>
public class TreeHarvestController : MonoBehaviour {

    [Tooltip("The minimum hits a tree's health can spawn with")]
    public int TreeMinHits = 3;

    [Tooltip("The maximum hits a tree's health can spawn with")]
    public int TreeMaxHits = 5;

    List<HarvestTreeInformation> hitTrees = new List<HarvestTreeInformation>();
    LootTable lootTable;
    Item playerEquippedItem;

    private void Start() {
        lootTable = GetComponent<LootTable>();

        Events.onTreeHit += onTreeHit;
        Events.onHotBarSlotSelected += onHotbarSelected;
    }

    private void onHotbarSelected(Item item) {
        playerEquippedItem = item;
    }

    private void onTreeHit(TreeInstance treeInstance) {
        if (playerEquippedItem != null && playerEquippedItem.ResourceGatherTypes.Contains(ResourceGatherType.Wood)) {
            var foundTree = hitTrees.Find(tree => tree.TreeInstance.prototypeIndex == treeInstance.prototypeIndex);

            if (foundTree == null) {
                var newTreeInformation = new HarvestTreeInformation {
                    Health = UnityEngine.Random.Range(TreeMinHits, TreeMaxHits + 1),
                    TreeInstance = treeInstance
                };
                hitTrees.Add(newTreeInformation);
                foundTree = newTreeInformation;
            }

            foundTree.Health--;

            LootAPI.GenerateLoot(lootTable, playerEquippedItem.ResourceGatheringEffectiveness);

            if (foundTree.Health <= 0) {
                Events.invokeOnTreeDestroyed(treeInstance);
                hitTrees.Remove(foundTree);
            }

        }
    }
}