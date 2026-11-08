using System.Linq;
using UnityEngine;

/// <summary>
/// This script is attached to the terrain, it handles tree related operations
/// </summary>
public class TerrainTreeController : MonoBehaviour {

    TreeInstance[] trees;
    TreeInstance[] initialTrees;
    Terrain terrain;
    TerrainCollider terrainCollider;

    private void Start() {
        Events.onAxeHitTree += onAxeHitTree;
        Events.onTreeDestroyed += onTreeDestroyed;

        terrain = GetComponent<Terrain>();
        trees = terrain.terrainData.treeInstances;
        initialTrees = terrain.terrainData.treeInstances;
        terrainCollider = GetComponent<TerrainCollider>();
    }

    private void onTreeDestroyed(TreeInstance treeInstance) {
        removeTree(treeInstance);
    }

    private void onAxeHitTree(RaycastHit raycastHit) {
        if (getNearestTree(raycastHit, out var treeInstance)) {
            Events.invokeOnTreeHit(treeInstance);
        }
    }

    void removeTree(TreeInstance treeInstance) {
        var treeList = trees.ToList();
        treeList.Remove(treeInstance);
        trees = treeList.ToArray();
        terrainCollider.enabled = false;
        terrain.terrainData.SetTreeInstances(trees, true);
        terrainCollider.enabled = true;
    }

    private void OnApplicationQuit() {
        terrain.terrainData.treeInstances = initialTrees;
    }

    bool getNearestTree(RaycastHit raycastHit, out TreeInstance treeInstance) {
        treeInstance = new TreeInstance();

        var tree = raycastHit.collider.gameObject.GetComponent<Terrain>();

        if (tree.transform.IsChildOf(terrain.transform)) {
            var nearestTree = trees.ToList().Aggregate((closestTree, currentTree) => {
                var currentTreePosition = Vector3.Scale(currentTree.position, terrain.terrainData.size) + terrain.transform.position;
                var closestTreePosition = Vector3.Scale(closestTree.position, terrain.terrainData.size) + terrain.transform.position;

                if (Vector3.Distance(currentTreePosition, raycastHit.point) < Vector3.Distance(closestTreePosition, raycastHit.point)) {
                    return currentTree;
                }
                return closestTree;
            });

            treeInstance = nearestTree;
            return true;
        }

        return false;
    }

}
