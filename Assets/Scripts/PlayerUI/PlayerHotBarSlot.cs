using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is attached to each hotbar slot inside the player hotbar, it listens for events invokes from the hotbar in order to
/// render the appropriate item information, it handles the ui for showing the item image, name, amount, and selected state of the hotbar slot
/// </summary>
public class PlayerHotBarSlot : MonoBehaviour {

    [Tooltip("The index of the hotbar slot in the hotbar")]
    public int HotBarSlotIndex;

    [Tooltip("The UI element indicating whether the slot is selected or not")]
    public Image SelectedBorder;

    [Tooltip("The ui component that parents the draggable item")]
    public GameObject DraggableItemParent;

    [Tooltip("The draggable item prefab")]
    public GameObject DraggableItem;

    [Tooltip("When the game starts, the hotbar slot will generate this item and place it inside the slot")]
    public ItemSubType StartFilledWith;

    DraggableItemsContainer draggableItemsContainer;

    Item item => draggableItemsContainer.Items.Count > 0 ? draggableItemsContainer.Items.First() : null;
    bool isSelected;

    private void Start() {
        draggableItemsContainer = GetComponentInChildren<DraggableItemsContainer>();

        Events.onItemRemovedFromDraggablesContainer += onItemRemoved;
        Events.onPlayerPlacedObject += onPlayerPlacedObject;

        if (StartFilledWith != ItemSubType.Empty) {
            CreateAndPlaceItem(StartFilledWith);
        }
    }

    private void onItemRemoved(Item removedItem, DraggableItemsContainer container) {
        if (container == draggableItemsContainer) {
            isSelected = false;
            Events.invokeOnHotBarSlotDeselected(removedItem);
            render();
        }
    }

    void CreateAndPlaceItem(ItemSubType itemSubType) {
        var newItem = ItemAPI.CreateItem(itemSubType);
        draggableItemsContainer.Items.Add(newItem);
    }

    private void Update() {
        var previouslySelected = isSelected;

        if (!GameState.IsMouseActive) {
            if (InputExtensions.IsNumeric()) {
                if (Input.GetKeyDown(KeyCode.Alpha1) && HotBarSlotIndex == 0 ||
                    Input.GetKeyDown(KeyCode.Alpha2) && HotBarSlotIndex == 1 ||
                    Input.GetKeyDown(KeyCode.Alpha3) && HotBarSlotIndex == 2 ||
                    Input.GetKeyDown(KeyCode.Alpha4) && HotBarSlotIndex == 3 ||
                    Input.GetKeyDown(KeyCode.Alpha5) && HotBarSlotIndex == 4 ||
                    Input.GetKeyDown(KeyCode.Alpha6) && HotBarSlotIndex == 5 ||
                    Input.GetKeyDown(KeyCode.Alpha7) && HotBarSlotIndex == 6 ||
                    Input.GetKeyDown(KeyCode.Alpha8) && HotBarSlotIndex == 7 ||
                    Input.GetKeyDown(KeyCode.Alpha9) && HotBarSlotIndex == 8 ||
                    Input.GetKeyDown(KeyCode.Alpha0) && HotBarSlotIndex == 9) {
                    isSelected = !isSelected;
                } else {
                    isSelected = false;
                }

                if (previouslySelected != isSelected && item != null) {
                    if (isSelected) {
                        Events.invokeOnHotBarSlotSelected(item);
                    } else {
                        Events.invokeOnHotBarSlotDeselected(item);
                    }

                    render();
                }
            }
        }
    }

    void onPlayerPlacedObject(Item itemPlaced) {
        if (itemPlaced == item) {
            item.Amount--;
        }
    }
    void render() {
        SelectedBorder.gameObject.SetActive(isSelected);
    }

}
