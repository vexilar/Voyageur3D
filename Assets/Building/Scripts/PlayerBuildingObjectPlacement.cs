using System.Linq;
using UnityEngine;

/// <summary>
/// This script is responisble for placing the players preview object in the right spot, it raycasts to do that
/// Its attached to the Player building object placement object, it moves the preview of what they are trying to build around in the world
/// </summary>
public class PlayerBuildingObjectPlacement : MonoBehaviour {

    [Tooltip("This is the object representing where the player is looking")]
    public Transform PlayerTransform;

    [Tooltip("The maximum distance an object can be built, distance is measured from the player to where he looking")]
    public float MaxPlacementDistance = 10f;

    PlayerPlacableObjectPreview activePreviewObject;

    private void Start() {
        GetComponentsInChildren<PlayerPlacableObjectPreview>(true).ToList().ForEach(preview => {
            preview.onPreviewSelected += onPreviewSelected;
        });
    }

    /// <summary>
    /// When the player selects a preview (which is an object he intends to build, and is previewing what he wants to place and where)
    /// </summary>
    /// <param name="playerPlacableObjectPreview">the preview object the player has selected</param>
    void onPreviewSelected(PlayerPlacableObjectPreview playerPlacableObjectPreview) {
        activePreviewObject = playerPlacableObjectPreview;
    }

    private void Update() {
        if (activePreviewObject) {
            var hit = Physics.Raycast(PlayerTransform.position, PlayerTransform.forward, out var raycast, MaxPlacementDistance, LayerMasks.Default | LayerMasks.PlayerBuilding);

            if (hit) {
                var didSnap = SnapToPlayerBuilding(raycast.collider.gameObject, raycast.point);

                if (!didSnap) {
                    transform.forward = transform.position.direction(PlayerTransform.position);

                    transform.forward = new Vector3(transform.forward.x, 0, transform.forward.z);
                    transform.position = raycast.point;
                }
            }
        }
    }

    /// <summary>
    /// This sets the position of this game object's transform to that of the nearest 'connector'
    /// Connectors are declared inside of Player Placed Objects, and define what can be placed against that object, and where they can be placed
    /// In the case there are multiple, which is common, it picks the one that is closest to where the player is looking
    /// </summary>
    /// <param name="raycastTarget">the gameobject of the collider the raycast hit</param>
    /// <param name="raycastHitPosition">the point the raycast hit that collider</param>
    /// <returns>a boolean indicating if it found a valid snap position</returns>
    bool SnapToPlayerBuilding(GameObject raycastTarget, Vector3 raycastHitPosition) {
        var placedObject = raycastTarget.GetComponentInParent<PlayerPlacedObject>();
        activePreviewObject.PlayerPlacedObjectedAgainst = placedObject;

        if (placedObject) {
            var connectorsForThisBuildingType = placedObject.Connectors.FindAll(connector => connector.PlayerBuildableType == activePreviewObject.PlayerBuildableType);

            if (connectorsForThisBuildingType.Count > 0) {
                var closestConnector = connectorsForThisBuildingType.Aggregate((currectClosestConnector, connector) => {
                    // loop through the list, comparing each connector's distance to the raycast hit position, the closest connector is chosen
                    if (currectClosestConnector == null || Vector3.Distance(connector.transform.position, raycastHitPosition) < Vector3.Distance(currectClosestConnector.transform.position, raycastHitPosition)) {
                        return connector;
                    } else {
                        return currectClosestConnector;
                    }
                });

                activePreviewObject.ConnectorFound = true;
                transform.position = closestConnector.transform.position;
                transform.rotation = closestConnector.transform.rotation;
                return true;
            }
        }

        activePreviewObject.ConnectorFound = false;
        return false;
    }

}
