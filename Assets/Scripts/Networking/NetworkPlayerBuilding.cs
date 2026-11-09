using Unity.Netcode;
using UnityEngine;

namespace Voyageur.Networking
{
    /// <summary>
    /// Owner requests placement; server spawns networked buildables for all clients.
    /// </summary>
    public class NetworkPlayerBuilding : NetworkBehaviour
    {
        [SerializeField] float maxPlaceDistance = 12f;

        public void RequestPlace(ItemSubType itemSubType, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (!IsOwner)
            {
                return;
            }

            RequestPlaceServerRpc(itemSubType, position, rotation, scale);
        }

        [ServerRpc]
        void RequestPlaceServerRpc(
            ItemSubType itemSubType,
            Vector3 position,
            Quaternion rotation,
            Vector3 scale,
            ServerRpcParams rpcParams = default)
        {
            if (!IsServer)
            {
                return;
            }

            if (!NetworkBuildingCatalog.TryGetPrefab(itemSubType, out var prefab))
            {
                Debug.LogWarning($"[NetworkPlayerBuilding] No networked prefab for {itemSubType}");
                return;
            }

            var sender = rpcParams.Receive.SenderClientId;
            if (NetworkManager.ConnectedClients.TryGetValue(sender, out var client) &&
                client.PlayerObject != null)
            {
                float distance = Vector3.Distance(client.PlayerObject.transform.position, position);
                if (distance > maxPlaceDistance)
                {
                    Debug.LogWarning(
                        $"[NetworkPlayerBuilding] Rejected place from client {sender}: distance {distance:F1}");
                    return;
                }
            }

            var instance = Instantiate(prefab, position, rotation);
            instance.transform.localScale = scale;

            var networkObject = instance.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogError($"[NetworkPlayerBuilding] Prefab {prefab.name} missing NetworkObject");
                Destroy(instance);
                return;
            }

            networkObject.Spawn(true);
            Debug.Log($"[NetworkPlayerBuilding] Spawned {prefab.name} for client {sender} at {position}");
        }
    }
}
