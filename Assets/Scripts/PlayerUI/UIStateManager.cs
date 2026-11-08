using UnityEngine;

/// <summary>
/// This script is attached to the canvas, it manages the ui state, things like whether the ui is open or not
/// </summary>
public class UIStateManager : MonoBehaviour {

    private void Start() {
        Events.onToggleMouseActive += onToggleMouseActive;
        Events.onPlayerOpenedContainer += onPlayerOpenedContainer;

        Events.onPlayerClickedToggleCraftingButton += onPlayerClickedToggleCraftingButton;
        Events.onPlayerClickedToggleInventoryButton += onPlayerClickedToggleInventoryButton;
    }

    private void onPlayerClickedToggleInventoryButton() {
        toggleInventory();
    }

    private void onPlayerClickedToggleCraftingButton() {
        toggleCraftingUI();
    }

    private void onPlayerOpenedContainer(WorldContainer obj) {
        Events.invokeToggleMouseActive(true);
        Events.invokeOnToggleInventory(true);
    }

    private void onToggleMouseActive(bool isMouseActive) {
        GameState.IsMouseActive = isMouseActive;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Events.invokeToggleMouseActive(!GameState.IsMouseActive);
            closeAllUI();
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            toggleInventory();
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            toggleCraftingUI();
        }
    }

    void toggleCraftingUI() {
        Events.invokeOnToggleInventory(false);
        Events.invokeToggleMouseActive(!GameState.IsCraftingUIOpen);
        Events.invokeOnPlayerToggleCraftingUI(!GameState.IsCraftingUIOpen);
    }

    void toggleInventory() {
        Events.invokeOnPlayerToggleCraftingUI(false);
        Events.invokeToggleMouseActive(!GameState.IsPlayerInventoryOpen);
        Events.invokeOnToggleInventory(!GameState.IsPlayerInventoryOpen);
    }

    void closeAllUI() {
        Events.invokeOnToggleInventory(false);
        Events.invokeOnPlayerToggleCraftingUI(false);
    }

}
