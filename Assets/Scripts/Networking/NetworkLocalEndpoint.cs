using UnityEngine;

namespace Voyageur.Networking
{
    /// <summary>
    /// Shared local Host/Client endpoint for Editor/MPPM. Prevents Multiplayer Play Mode
    /// clones from using a stale in-memory scene port after the main Editor saves a new one.
    /// </summary>
    [CreateAssetMenu(menuName = "Voyageur/Networking/Local Endpoint", fileName = "NetworkLocalEndpoint")]
    public class NetworkLocalEndpoint : ScriptableObject
    {
        public const string ResourcesPath = "NetworkLocalEndpoint";

        [SerializeField] string address = "127.0.0.1";
        [SerializeField] ushort port = 7779;

        public string Address => address;
        public ushort Port => port;

        public static bool TryLoad(out NetworkLocalEndpoint endpoint)
        {
            endpoint = Resources.Load<NetworkLocalEndpoint>(ResourcesPath);
            return endpoint != null;
        }
    }
}
