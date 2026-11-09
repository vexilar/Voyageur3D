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
        [SerializeField] ushort port = 7779;
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

            ApplySharedLocalEndpointIfPresent();
            role = ResolveRuntimeRole();
            ApplyTransportEndpoint();
        }

        void ApplySharedLocalEndpointIfPresent()
        {
            // MPPM clones often keep a stale in-memory scene after the main Editor saves a
            // new port. A Resources asset is always loaded fresh and keeps Host/Client aligned.
            if (!NetworkLocalEndpoint.TryLoad(out var endpoint))
            {
                return;
            }

            address = endpoint.Address;
            port = endpoint.Port;
        }

        NetworkRuntimeRole ResolveRuntimeRole()
        {
            if (TryGetPortFromCommandLine(out var portFromArgs))
            {
                port = portFromArgs;
            }

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

        static bool TryGetPortFromCommandLine(out ushort parsed)
        {
            parsed = 0;
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (!string.Equals(args[i], "-voyageur-port", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (ushort.TryParse(args[i + 1], out parsed) && parsed > 0)
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

            if (!started)
            {
                string hint = runtimeRole == NetworkRuntimeRole.Client
                    ? "Is the Host listening on this address/port?"
                    : "UDP port is likely already bound by another Unity/Voyageur Host. Stop the other instance and retry.";
                Debug.LogError(
                    $"[NetworkBootstrap] Failed to start {runtimeRole} @ {address}:{port}. {hint}");
            }

            status = started
                ? $"Started {runtimeRole} @ {address}:{port}"
                : $"Failed to start {runtimeRole} @ {address}:{port}";
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

        void OnDisable()
        {
            // Leaving Play Mode must release the UDP listen socket; otherwise the next
            // Host start fails with "Server failed to bind" on the same Editor process.
            ShutdownNetworking();
        }

        void OnApplicationQuit()
        {
            ShutdownNetworking();
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

            // Listen on the same loopback address clients use for local Editor/MPPM.
            transport.SetConnectionData(address, port, address);
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
            // MPPM virtual players always live under Library/VP/<id>/. This is the most
            // reliable signal when CurrentPlayer reflection fails across package versions.
            var dataPath = Application.dataPath.Replace('\\', '/');
            if (dataPath.IndexOf("/Library/VP/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return false;
            }

            try
            {
                var type = FindCurrentPlayerType();
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

        static Type FindCurrentPlayerType()
        {
            // Unity 6.x ships this on UnityEngine.MultiplayerModule with capital "Mode".
            string[] assemblyQualifiedNames =
            {
                "Unity.Multiplayer.PlayMode.CurrentPlayer, UnityEngine.MultiplayerModule",
                "Unity.Multiplayer.Playmode.CurrentPlayer, Unity.Multiplayer.Playmode",
                "Unity.Multiplayer.Playmode.CurrentPlayer, Unity.Multiplayer.Playmode.Runtime",
                "Unity.Multiplayer.Playmode.CurrentPlayer, Unity.Multiplayer.Playmode.Common.Runtime"
            };

            for (int i = 0; i < assemblyQualifiedNames.Length; i++)
            {
                var type = Type.GetType(assemblyQualifiedNames[i]);
                if (type != null)
                {
                    return type;
                }
            }

            string[] typeNames =
            {
                "Unity.Multiplayer.PlayMode.CurrentPlayer",
                "Unity.Multiplayer.Playmode.CurrentPlayer"
            };

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                for (int i = 0; i < typeNames.Length; i++)
                {
                    Type type = null;
                    try
                    {
                        type = assembly.GetType(typeNames[i], false);
                    }
                    catch
                    {
                        // Ignored — some dynamic assemblies throw on GetType.
                    }

                    if (type != null)
                    {
                        return type;
                    }
                }
            }

            return null;
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
