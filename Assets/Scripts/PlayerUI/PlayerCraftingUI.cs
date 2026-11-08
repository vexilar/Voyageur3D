using UnityEngine;

/// <summary>
/// This script is attached to the player crafting ui in the canvas, it handles rendering the items the player can craft, it uses the item definitions which is set elsewhere 
/// and renders a crafting group for each type of item
/// </summary>
public class PlayerCraftingUI : MonoBehaviour {

    [Tooltip("The Container to toggle when the player enables and disables the crafting UI")]
    public GameObject ToggleContainer;

    [Tooltip("This is the container that the craftable groups are instantiated under")]
    public GameObject CraftablesContainer;

    [Tooltip("This is the prefab that represents a group of craftable items in the ui")]
    public GameObject CraftingGroupComponent;

    private void Start() {
        Events.onPlayerToggleCraftingUI += onPlayerToggleCraftingUI;
        render();
    }

    private void onPlayerToggleCraftingUI(bool shouldOpen) {
        GameState.IsCraftingUIOpen = shouldOpen;
        ToggleContainer.SetActive(shouldOpen);
    }

    void render() {
        populateCraftableGroups();
    }

    void populateCraftableGroups() {
        EnumUtil.GetValues<ItemType>().ForEach(type => {
            var instance = Instantiate(CraftingGroupComponent, CraftablesContainer.transform);
            var craftingGroup = instance.GetComponent<CraftingGroup>();
            
            if (craftingGroup) {
                craftingGroup.ItemType = type;
            }
        });
    }
}
