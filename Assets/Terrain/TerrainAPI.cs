using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This api exists to manage requests against the terrain controllers
/// </summary>
public static class TerrainAPI {

    static List<TerrainController> terrains = new List<TerrainController>();

    public static void AddTerrain(TerrainController terrain) {
        terrains.Add(terrain);
    }

    public static bool RaycastForTree(RaycastRequest raycastRequest, out RaycastHit raycastHit) {
        raycastHit = new RaycastHit();

        var results = terrains.Select(terrain => terrain.RaycastForTree(raycastRequest));

        var result = results.ToList().Find(r => r.DidHit);

        if (result != null) {
            raycastHit = result.HitInformation;
        }

        return result != null;
    }

}
