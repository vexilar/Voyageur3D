using Unity.Netcode;
using UnityEngine;

namespace Voyageur.Networking
{
    /// <summary>
    /// Disables the scene-placed FPSController once networking is listening,
    /// so the spawned NetworkPlayer is the avatar you control/see.
    /// </summary>
    public class DisableSceneFpsWhenNetworked : MonoBehaviour
    {
        [SerializeField] string fpsObjectName = "FPSController";

        void Update()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null || !nm.IsListening)
            {
                return;
            }

            var fps = GameObject.Find(fpsObjectName);
            if (fps != null && fps.activeSelf)
            {
                fps.SetActive(false);
                Debug.Log($"[DisableSceneFpsWhenNetworked] Disabled scene '{fpsObjectName}' for networked play.");
            }

            enabled = false;
        }
    }
}
