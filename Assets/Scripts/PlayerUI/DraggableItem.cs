using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is attached to the draggable item that is movable between item containers
/// </summary>
public class DraggableItem : MonoBehaviour {

    [Tooltip("This is the image element that shows the item image")]
    public Image ItemImage;

    [Tooltip("This is the text element that shows the amount of the item in this slot")]
    public TextMeshProUGUI ItemAmount;

    Draggable draggable;
    DraggableItemsContainer draggableItemsContainer;
    TooltipTrigger tooltipTrigger;

    private void Awake() {
        draggableItemsContainer = GetComponentInParent<DraggableItemsContainer>();
    }

    public void RenderItem(Item item) {
        if (!draggable) {
            draggable = GetComponent<Draggable>();
            registerDraggableEvents(item);
        }

        if (!tooltipTrigger) {
            tooltipTrigger = GetComponent<TooltipTrigger>();
            tooltipTrigger.TooltipTriggerInformation.Item = item;
        }

        ItemImage.sprite = item.Sprite;

        var itemAmount = item.IsStackable ? item.Amount.ToString() : "";
        ItemAmount.text = itemAmount;
    }

    void registerDraggableEvents(Item item) {
        draggable.onEndDrag.AddListener(() => {
            GameState.DraggedItem = null;

            if (GameState.DraggedItemDroppedOnContainer) {
                Events.invokeOnItemRemovedFromDraggablesContainer(item, draggableItemsContainer);
                GameState.DraggedItemDroppedOnContainer = false;
                Destroy(gameObject);
            }
        });

        draggable.onBeginDrag.AddListener(() => {
            GameState.DraggedItemOrigin = draggableItemsContainer;
            GameState.DraggedItem = item;
        });
    }
}
