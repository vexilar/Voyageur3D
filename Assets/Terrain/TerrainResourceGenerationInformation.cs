using System;
using UnityEngine;

/// <summary>
/// This class is used to identify which item to generate and how many from the terrain resource generator
/// </summary>
[Serializable]
public class TerrainResourceGenerationInformation {

    [Tooltip("The minimum amount of this prefab to generate")]
    public int MinimumAmount;

    [Tooltip("The maximum amount of this prefab to generate")]
    public int MaximumAmount;

    [Tooltip("The prefab to instantiate")]
    public GameObject Prefab;

}
