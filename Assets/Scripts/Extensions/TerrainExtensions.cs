using UnityEngine;

/// <summary>
/// This extends the terrain class to have additional methods
/// </summary>
public static class TerrainExtensions {

    public static Vector3 GetRandomPosition(this Terrain terrain) {
        var bounds = terrain.terrainData.bounds;
        var minX = bounds.center.x - bounds.extents.x;
        var maxX = bounds.center.x + bounds.extents.x;
        var minZ = bounds.center.z - bounds.extents.z;
        var maxZ = bounds.center.z + bounds.extents.z;

        var xPosition = Random.Range(minX, maxX);
        var zPosition = Random.Range(minZ, maxZ);
        var yPosition = terrain.SampleHeight(new Vector3(xPosition, 0, zPosition));

        var randomPosition = new Vector3(xPosition, yPosition, zPosition);
        return randomPosition;
    }

}