using Unity.Netcode;
using UnityEngine;

namespace Voyageur.Networking
{
    /// <summary>
    /// Moves newly spawned players to configured spawn points on the server.
    /// NGO defaults to (0,0,0), which is the terrain corner in Lukes Scene.
    /// </summary>
    public class NetworkPlayerSpawnPoints : MonoBehaviour
    {
        [SerializeField] Vector3[] spawnPositions =
        {
            new Vector3(250f, 30.5f, 250f),
            new Vector3(254f, 30.5f, 250f),
            new Vector3(250f, 30.5f, 254f),
            new Vector3(254f, 30.5f, 254f)
        };

        NetworkManager networkManager;

        void Awake()
        {
            networkManager = GetComponent<NetworkManager>();
        }

        void OnEnable()
        {
            if (networkManager == null)
            {
                networkManager = NetworkManager.Singleton;
            }

            if (networkManager != null)
            {
                networkManager.OnClientConnectedCallback += HandleClientConnected;
            }
        }

        void OnDisable()
        {
            if (networkManager != null)
            {
                networkManager.OnClientConnectedCallback -= HandleClientConnected;
            }
        }

        void HandleClientConnected(ulong clientId)
        {
            if (networkManager == null || !networkManager.IsServer)
            {
                return;
            }

            // Host fires this for itself before the player object always exists; defer one frame.
            StartCoroutine(MovePlayerNextFrame(clientId));
        }

        System.Collections.IEnumerator MovePlayerNextFrame(ulong clientId)
        {
            yield return null;

            if (!networkManager.ConnectedClients.TryGetValue(clientId, out var client) ||
                client.PlayerObject == null)
            {
                yield break;
            }

            int index = 0;
            if (spawnPositions != null && spawnPositions.Length > 0)
            {
                index = (int)(clientId % (ulong)spawnPositions.Length);
                var pos = spawnPositions[index];
                var player = client.PlayerObject;
                player.transform.position = pos;

                var cc = player.GetComponent<CharacterController>();
                if (cc != null)
                {
                    cc.enabled = false;
                    player.transform.position = pos;
                    cc.enabled = true;
                }

                Debug.Log($"[NetworkPlayerSpawnPoints] client={clientId} -> {pos}");
            }
        }
    }
}
