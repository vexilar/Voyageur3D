using UnityEngine;

public static class HarvestAPI {

    public static bool HarvestResource(RaycastRequest raycastRequest, out RaycastHit raycastHit) {
        raycastRequest.OnHit += onRaycast;
        var response = raycastRequest.Raycast();
        
        raycastHit = response.HitInformation;
        return response.DidHit;
    }

    static RaycastResponse onRaycast(RaycastHit raycastHit) {
        var response = new RaycastResponse { HitInformation = raycastHit };

        var harvestableResource = raycastHit.collider.gameObject.GetComponent<HarvestableResource>();

        if (harvestableResource != null) {
            response.DidHit = harvestableResource.ResourceHit();
        }

        return response;
    }

}
