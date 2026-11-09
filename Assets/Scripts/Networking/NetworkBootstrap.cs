using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Voyageur.Networking
{
    /// <summary>
    /// Single entry point for Host / Client / Server startup.
    /// Editor default is Host. Multiplayer Play Mode virtual players (when detectable)
    /// auto-start as Client against the configured address/port.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class NetworkBootstrap : MonoBehaviour
    {
        [SerializeField] NetworkRuntimeRole role = NetworkRuntimeRole.Host;
        [SerializeField] string address = "127.0.0.1";
        [SerializeField] ushort port = 7777;
        [SerializeField] bool autoStart = true;
        [SerializeField] bool showDebugGui = true;
        [SerializeField] bool preferClientWhenNotMainEditor = true;

        NetworkManager networkManager;
        string status = "Idle";

        void Awake()
        {
            networkManager = GetComponent<NetworkManager>();
            if (networkManager == null)
            {
                networkManager = FindFirstObjectByType<NetworkManager>();
            }

            role = ResolveRuntimeRole();
            ApplyTransportEndpoint();
        }

        NetworkRuntimeRole ResolveRuntimeRole()
        {
            if (TryGetRoleFromCommandLine(out var fromArgs))
            {
                return fromArgs;
            }

            // Standalone/local player builds used as second clients.
            if (!Application.isEditor)
            {
                return NetworkRuntimeRole.Client;
            }

            if (preferClientWhenNotMainEditor && !IsMainEditorInstance())
            {
                return NetworkRuntimeRole.Client;
            }

            return role;
        }

        static bool TryGetRoleFromCommandLine(out NetworkRuntimeRole parsed)
        {
            parsed = NetworkRuntimeRole.Host;
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (!string.Equals(args[i], "-voyageur-role", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (Enum.TryParse(args[i + 1], true, out parsed))
                {
                    return true;
                }
            }

            return false;
        }

        void Start()
        {
            if (!autoStart)
            {
                status = "Auto-start disabled";
                return;
            }

            StartNetworking(role);
        }

        public void StartNetworking(NetworkRuntimeRole runtimeRole)
        {
            if (networkManager == null)
            {
                status = "Missing NetworkManager";
                Debug.LogError("[NetworkBootstrap] No NetworkManager found.");
                return;
            }

            if (networkManager.IsListening)
            {
                status = $"Already running as {DescribeRole()}";
                return;
            }

            role = runtimeRole;
            ApplyTransportEndpoint();

            bool started = false;
            switch (runtimeRole)
            {
                case NetworkRuntimeRole.Host:
                    started = networkManager.StartHost();
                    break;
                case NetworkRuntimeRole.Client:
                    started = networkManager.StartClient();
                    break;
                case NetworkRuntimeRole.Server:
                    started = networkManager.StartServer();
                    break;
            }

            status = started
                ? $"Started {runtimeRole} @ {address}:{port}"
                : $"Failed to start {runtimeRole}";
            Debug.Log($"[NetworkBootstrap] role={runtimeRole} address={address}:{port} started={started}");
        }

        public void ShutdownNetworking()
        {
            if (networkManager != null && networkManager.IsListening)
            {
                networkManager.Shutdown();
                status = "Shutdown";
            }
        }

        void ApplyTransportEndpoint()
        {
            if (networkManager == null)
            {
                return;
            }

            var transport = networkManager.GetComponent<UnityTransport>();
            if (transport == null)
            {
                transport = networkManager.NetworkConfig?.NetworkTransport as UnityTransport;
            }

            if (transport == null)
            {
                Debug.LogError("[NetworkBootstrap] UnityTransport missing on NetworkManager.");
                return;
            }

            transport.SetConnectionData(address, port);
        }

        string DescribeRole()
        {
            if (networkManager == null)
            {
                return "None";
            }

            if (networkManager.IsHost)
            {
                return "Host";
            }

            if (networkManager.IsServer)
            {
                return "Server";
            }

            if (networkManager.IsClient)
            {
                return "Client";
            }

            return "Disconnected";
        }

        static bool IsMainEditorInstance()
        {
#if UNITY_EDITOR
            try
            {
                var type = Type.GetType("Unity.Multiplayer.Playmode.CurrentPlayer, Unity.Multiplayer.Playmode");
                if (type == null)
                {
                    type = Type.GetType("Unity.Multiplayer.Playmode.CurrentPlayer, Unity.Multiplayer.Playmode.Runtime");
                }

                var property = type?.GetProperty(
                    "IsMainEditor",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (property != null && property.PropertyType == typeof(bool))
                {
                    return (bool)property.GetValue(null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(
                    $"[NetworkBootstrap] Could not query Multiplayer Play Mode main editor flag: {ex.Message}");
            }
#endif
            return true;
        }

        void OnGUI()
        {
            if (!showDebugGui)
            {
                return;
            }

            const int width = 280;
            GUILayout.BeginArea(new Rect(12, 12, width, 220), GUI.skin.box);
            GUILayout.Label("Voyageur Network Bootstrap");
            GUILayout.Label($"Role: {DescribeRole()}");
            GUILayout.Label($"Endpoint: {address}:{port}");
            GUILayout.Label(status);

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Host"))
                {
                    ShutdownNetworking();
                    StartNetworking(NetworkRuntimeRole.Host);
                }

                if (GUILayout.Button("Client"))
                {
                    ShutdownNetworking();
                    StartNetworking(NetworkRuntimeRole.Client);
                }

                if (GUILayout.Button("Server"))
                {
                    ShutdownNetworking();
                    StartNetworking(NetworkRuntimeRole.Server);
                }
            }

            if (GUILayout.Button("Shutdown"))
            {
                ShutdownNetworking();
            }

            GUILayout.EndArea();
        }
    }
}
