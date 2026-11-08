using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is attached to a terrain, and generates the resources specified and places them randomly
/// </summary>
public class TerrainResourceGenerator : MonoBehaviour {

    [Tooltip("The resources to generate")]
    public List<TerrainResourceGenerationInformation> ResourceList = new List<TerrainResourceGenerationInformation>();

    Terrain terrain;

    private void Start() {
        terrain = GetComponent<Terrain>();

        createResources();
    }

    void createResources() {
        ResourceList.ForEach(resourceInfo => {
            var amountToGenerate = Random.Range(resourceInfo.MinimumAmount, resourceInfo.MaximumAmount + 1);

            for (int i = 0; i < amountToGenerate; i++) {
                var randomPosition = terrain.GetRandomPosition();

                Instantiate(resourceInfo.Prefab, randomPosition, Quaternion.identity);
            }
        });

    }

}
