using Unity.Netcode;
using UnityEngine;

namespace Voyageur.Networking
{
    /// <summary>
    /// Disables the scene-placed FPSController once the local NetworkPlayer camera is live.
    /// Waiting for the owner camera avoids a blank Game view (HUD-only) when the client is
    /// listening but has not spawned yet, or when Host failed to bind.
    /// </summary>
    public class DisableSceneFpsWhenNetworked : MonoBehaviour
    {
        [SerializeField] string fpsObjectName = "FPSController";
        [SerializeField] float warnAfterSeconds = 15f;

        float listenStartedAt = -1f;
        bool loggedWaiting;
        bool loggedTimeoutWarning;

        void Update()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null || !nm.IsListening)
            {
                listenStartedAt = -1f;
                loggedWaiting = false;
                loggedTimeoutWarning = false;
                return;
            }

            if (listenStartedAt < 0f)
            {
                listenStartedAt = Time.unscaledTime;
            }

            // Dedicated server has no local player camera to wait for.
            if (nm.IsServer && !nm.IsClient)
            {
                DisableSceneFps("server-only");
                return;
            }

            if (TryGetLocalOwnerCamera(out var ownerCamera) && ownerCamera.enabled)
            {
                DisableSceneFps($"owner camera ready ({ownerCamera.name})");
                return;
            }

            if (!loggedWaiting)
            {
                loggedWaiting = true;
                Debug.Log(
                    "[DisableSceneFpsWhenNetworked] Networking is listening; waiting for local NetworkPlayer camera before disabling scene FPS.");
            }

            if (!loggedTimeoutWarning && Time.unscaledTime - listenStartedAt >= warnAfterSeconds)
            {
                loggedTimeoutWarning = true;
                Debug.LogWarning(
                    "[DisableSceneFpsWhenNetworked] Still waiting for NetworkPlayer camera; leaving scene FPS active so the view is not blank.");
            }
        }

        static bool TryGetLocalOwnerCamera(out Camera ownerCamera)
        {
            ownerCamera = null;
            var setups = FindObjectsByType<NetworkPlayerOwnerSetup>(FindObjectsInactive.Exclude);
            for (int i = 0; i < setups.Length; i++)
            {
                var setup = setups[i];
                if (setup == null || !setup.IsSpawned || !setup.IsOwner)
                {
                    continue;
                }

                ownerCamera = setup.GetComponentInChildren<Camera>(true);
                if (ownerCamera != null)
                {
                    return true;
                }
            }

            return false;
        }

        void DisableSceneFps(string reason)
        {
            var fps = GameObject.Find(fpsObjectName);
            if (fps != null && fps.activeSelf)
            {
                fps.SetActive(false);
                Debug.Log(
                    $"[DisableSceneFpsWhenNetworked] Disabled scene '{fpsObjectName}' ({reason}).");
            }

            enabled = false;
        }
    }
}
