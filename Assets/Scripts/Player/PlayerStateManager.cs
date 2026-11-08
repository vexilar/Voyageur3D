using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// This script is attached to the fps controller, it disables the player during certain events
/// </summary>
public class PlayerStateManager : MonoBehaviour {

    FirstPersonController firstPersonController;

    private void Start() {
        firstPersonController = GetComponent<FirstPersonController>();

        Events.onToggleMouseActive += onToggleMouseActive;
    }

    private void onToggleMouseActive(bool isMouseActive) {
        firstPersonController.enabled = !isMouseActive;
    }

}
