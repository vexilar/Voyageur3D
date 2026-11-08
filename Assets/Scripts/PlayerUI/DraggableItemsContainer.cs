using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This script is attached to ui elements that contain items, it captures dragged items, and keeps track of them, it renders the draggable items in the container
/// </summary>
public class DraggableItemsContainer : MonoBehaviour, IDropHandler {

    public event Action<List<Item>> onItemsChanged;

    [Tooltip("This is the prefab that represents an item in a container")]
    public GameObject DraggableItemPrefab;

    [Tooltip("This is the object that contains the draggable items")]
    public Transform ItemsContainer;

    [Tooltip("The maximum amount of items that can fit into this container, use 0 for infinite")]
    public int MaxSlots;

    [NonSerialized]
    public ObservableCollection<Item> Items = new ObservableCollection<Item>();

    [NonSerialized]
    public WorldContainer WorldContainer;

    private void Start() {
        Events.onItemRemovedFromDraggablesContainer += onItemRemovedFromDraggablesContainer;
        Items.CollectionChanged += Items_CollectionChanged;
    }

    private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        onItemsChanged?.Invoke(Items.ToList());

        if (WorldContainer) {
            Events.invokeOnWorldContainerUpdated(WorldContainer, Items.ToList());
        }

        render();
    }

    void onItemRemovedFromDraggablesContainer(Item item, DraggableItemsContainer draggableItemsContainer) {
        if (draggableItemsContainer == this) {
            Items.Remove(item);
        }
    }

    public void OnDrop(PointerEventData eventData) {
        var maxSlotsReached = MaxSlots > 0 && Items.Count >= MaxSlots;

        if (maxSlotsReached) {
            swapItem();
        }
        Items.Add(GameState.DraggedItem);

        GameState.DraggedItemDroppedOnContainer = true;
    }

    void swapItem() {
        var itemToSwap = Items.Last();

        if (GameState.DraggedItemOrigin) {
            GameState.DraggedItemOrigin.Items.Add(itemToSwap);
        }

        Items.Remove(itemToSwap);
    }

    void render() {
        removeItems();
        populateItems();
    }

    void populateItems() {
        Items.ToList().ForEach(item => {
            var instance = Instantiate(DraggableItemPrefab, ItemsContainer.transform);
            var containerItem = instance.GetComponent<DraggableItem>();

            if (containerItem) {
                containerItem.RenderItem(item);
            }
        });
    }

    void removeItems() {
        foreach (Transform child in ItemsContainer) {
            Destroy(child.gameObject);
        }
    }

}
