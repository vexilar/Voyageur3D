using UnityEngine;

/// <summary>
/// This script is attached to each terrain in the game, it handles operations against the terrain
/// </summary>
public class TerrainController : MonoBehaviour {

    Terrain terrain;

    private void Start() {
        terrain = GetComponent<Terrain>();

        TerrainAPI.AddTerrain(this);
    }

    public RaycastResponse RaycastForTree(RaycastRequest raycastRequest) {
        raycastRequest.OnHit = onTreeHit;
        return raycastRequest.Raycast();
    }

    RaycastResponse onTreeHit(RaycastHit raycastHit) {
        var terrainHit = raycastHit.collider.gameObject;
        var hitPosition = raycastHit.point;

        var response = new RaycastResponse { DidHit = false };

        if (terrainHit.transform.IsChildOf(terrain.transform)) {
            var groundPosition = new Vector3(hitPosition.x, terrain.SampleHeight(hitPosition), hitPosition.z);

            if (Vector3.Distance(groundPosition, hitPosition) > .5f) {
                response.DidHit = true;
                response.HitInformation = raycastHit;
                return response;
            }
        }
        return response;
    }
}
