using Unity.Netcode;
using UnityEngine;

namespace Voyageur.Networking
{
    /// <summary>
    /// Enables owner-only camera / audio and locks the cursor for local play.
    /// </summary>
    public class NetworkPlayerOwnerSetup : NetworkBehaviour
    {
        [SerializeField] Camera ownerCamera;
        [SerializeField] AudioListener ownerAudioListener;
        [SerializeField] Behaviour[] ownerOnlyBehaviours;
        [SerializeField] bool lockCursorForOwner = true;

        public override void OnNetworkSpawn()
        {
            bool isOwner = IsOwner;
            if (ownerCamera != null)
            {
                ownerCamera.enabled = isOwner;
            }

            if (ownerAudioListener != null)
            {
                ownerAudioListener.enabled = isOwner;
            }

            if (ownerOnlyBehaviours != null)
            {
                for (int i = 0; i < ownerOnlyBehaviours.Length; i++)
                {
                    if (ownerOnlyBehaviours[i] != null)
                    {
                        ownerOnlyBehaviours[i].enabled = isOwner;
                    }
                }
            }

            if (isOwner)
            {
                Events.onToggleMouseActive += OnToggleMouseActive;
                if (ownerCamera != null)
                {
                    ownerCamera.tag = "MainCamera";
                }

                if (lockCursorForOwner && !GameState.IsMouseActive)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }

            Debug.Log(
                $"[NetworkPlayerOwnerSetup] clientId={OwnerClientId} isOwner={isOwner} isServer={IsServer}");
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
            {
                Events.onToggleMouseActive -= OnToggleMouseActive;
                if (lockCursorForOwner)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        void OnToggleMouseActive(bool isActive)
        {
            if (!IsOwner || !lockCursorForOwner)
            {
                return;
            }

            Cursor.visible = isActive;
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
        }

        void Update()
        {
            if (!IsSpawned || !IsOwner || !lockCursorForOwner)
            {
                return;
            }

            // Crafting / inventory owns the cursor while the mouse is active.
            if (GameState.IsMouseActive)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
