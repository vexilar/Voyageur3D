using TMPro;
using UnityEngine;

/// <summary>
/// This script is attached to a ui element in the canvas that controls the world container interface (world container's are containers in the world like a box or enemy corpse)
/// This script handles mostly wiring of showing and hiding the interface, as well as making sure it has the right items and text to describe the world container
/// </summary>
public class WorldContainerInterface : MonoBehaviour {

    [Tooltip("This is the child object that you want to toggle on and off")]
    public GameObject ContainerToToggle;

    [Tooltip("This is the text component that displays the container name")]
    public TextMeshProUGUI ContainerName;

    DraggableItemsContainer draggableItemsContainer;

    private void Start() {
        Events.onPlayerOpenedContainer += onPlayerOpenedContainer;
        Events.onPlayerToggleInventory += onPlayerToggleInventory;

        draggableItemsContainer = GetComponent<DraggableItemsContainer>();
    }

    private void onPlayerToggleInventory(bool isOpen) {
        if (!isOpen) {
            ContainerToToggle.SetActive(false);
            ContainerName.text = "";
            draggableItemsContainer.WorldContainer = null;
            draggableItemsContainer.Items.Clear();
        }
    }

    void onPlayerOpenedContainer(WorldContainer worldContainer) {
        draggableItemsContainer.Items.Clear();
        worldContainer.Items.ForEach(item => draggableItemsContainer.Items.Add(item));
        ContainerToToggle.SetActive(true);
        ContainerName.text = worldContainer.Name;
        draggableItemsContainer.WorldContainer = worldContainer;
    }

}
