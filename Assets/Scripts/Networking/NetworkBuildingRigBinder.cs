using Unity.Netcode;
using UnityEngine;

namespace Voyageur.Networking
{
    /// <summary>
    /// For the owning client, points the scene building placement rig at this player's camera
    /// so preview/raycast works after the local FPSController is disabled.
    /// </summary>
    public class NetworkBuildingRigBinder : NetworkBehaviour
    {
        [SerializeField] Transform ownerCamera;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                return;
            }

            if (ownerCamera == null)
            {
                var cam = GetComponentInChildren<Camera>(true);
                if (cam != null)
                {
                    ownerCamera = cam.transform;
                }
            }

            var placement = FindAnyObjectByType<PlayerBuildingObjectPlacement>();
            if (placement == null)
            {
                Debug.LogWarning("[NetworkBuildingRigBinder] No PlayerBuildingObjectPlacement in scene");
                return;
            }

            if (ownerCamera != null)
            {
                placement.PlayerTransform = ownerCamera;
                Debug.Log("[NetworkBuildingRigBinder] Bound building placement to owner camera");
            }
        }
    }
}
