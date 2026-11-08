using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This class contains events used to communicate globalls between objects in the game
/// </summary>
public static class Events {

    /// <summary>
    /// This event is fired when an enemy is hit, it passes the enemy being hit, and the damage source that hit the enemy
    /// </summary>
    public static event Action<GameObject, DamageSource> onEnemyHit;

    public static void invokeOnEnemyHit(GameObject enemy, DamageSource damageSource) {
        onEnemyHit?.Invoke(enemy, damageSource);
    }

    /// <summary>
    /// This event is fired when a hotbar slot is selected, it passes the item contained in the slot being selected
    /// </summary>
    public static event Action<Item> onHotBarSlotSelected;

    public static void invokeOnHotBarSlotSelected(Item item) {
        onHotBarSlotSelected?.Invoke(item);
    }

    /// <summary>
    /// This event is fired when a hotbar slot is deselected, it passes the item contained in the slot being deselected
    /// </summary>
    public static event Action<Item> onHotBarSlotDeselected;

    public static void invokeOnHotBarSlotDeselected(Item item) {
        onHotBarSlotDeselected?.Invoke(item);
    }

    /// <summary>
    /// This event is fired when the player places a buildable object, it passes the item placed
    /// </summary>
    public static event Action<Item> onPlayerPlacedObject;

    public static void invokeOnPlayerPlacedObject(Item placedItem) {
        onPlayerPlacedObject?.Invoke(placedItem);
    }

    /// <summary>
    /// This events is fired when the player interacts with an object tagged as interactable, it passes the item he interacted with
    /// </summary>
    public static event Action<GameObject> onPlayerInteracted;

    public static void invokeOnPlayerInteracted(GameObject interactable) {
        onPlayerInteracted?.Invoke(interactable);
    }

    /// <summary>
    /// This events is fired when the player opens a container, it passes the world container being opened
    /// </summary>
    public static event Action<WorldContainer> onPlayerOpenedContainer;

    public static void invokeOnPlayerOpenedContainer(WorldContainer worldContainer) {
        onPlayerOpenedContainer?.Invoke(worldContainer);
    }

    /// <summary>
    /// This event is fired when the player removes an item from a draggables container, it passes the item being removed, and the container it was removed from
    /// </summary>
    public static event Action<Item, DraggableItemsContainer> onItemRemovedFromDraggablesContainer;

    public static void invokeOnItemRemovedFromDraggablesContainer(Item item, DraggableItemsContainer draggableItemsContainer) {
        onItemRemovedFromDraggablesContainer?.Invoke(item, draggableItemsContainer);
    }

    /// <summary>
    /// This event is fired when a world containers item's have changed, it helps notify the world container that it's items have changed when the player is interacting with
    /// the ui that represents the items inside of it in the canvas, it passes the instance of the world container being interacted with, and the list of items inside that container
    /// </summary>
    public static event Action<WorldContainer, List<Item>> onWorldContainerUpdated;

    public static void invokeOnWorldContainerUpdated(WorldContainer worldContainer, List<Item> items) {
        onWorldContainerUpdated?.Invoke(worldContainer, items);
    }

    /// <summary>
    /// This event is fired when the tooltip trigger is hovered over, notifying tooltips to display depending on the type, it passes information about the tooltip trigger
    /// </summary>
    public static event Action<TooltipTriggerInformation> onShowTooltip;

    public static void invokeOnShowTooltip(TooltipTriggerInformation tooltipTriggerInformation) {
        onShowTooltip?.Invoke(tooltipTriggerInformation);
    }

    /// <summary>
    /// This event is fired when the tooltip trigger is unhovered, telling the tooltip to hide
    /// </summary>
    public static event Action<TooltipTriggerInformation> onHideTooltip;

    public static void invokeOnHideTooltip(TooltipTriggerInformation tooltipTriggerInformation) {
        onHideTooltip?.Invoke(tooltipTriggerInformation);
    }

    /// <summary>
    /// This event is fired when the player axe raycasts to hit a tree, it passes the position the raycast hit when raycasting against trees
    /// </summary>
    public static event Action<RaycastHit> onAxeHitTree;

    public static void invokeOnAxeHitTree(RaycastHit raycastHit) {
        onAxeHitTree?.Invoke(raycastHit);
    }

    /// <summary>
    /// This events is fired when a tree is hit, it passes the tree instance
    /// </summary>
    public static event Action<TreeInstance> onTreeHit;

    public static void invokeOnTreeHit(TreeInstance treeInstance) {
        onTreeHit?.Invoke(treeInstance);
    }

    /// <summary>
    /// This event is invoked by item acquisition, and passes the item acquired
    /// </summary>
    public static event Action<Item> onItemAcquired;

    public static void invokeOnItemAcquired(Item item) {
        onItemAcquired?.Invoke(item);
    }

    /// <summary>
    /// This event is invoked when a tree is destroyed, it passes the tree instance
    /// </summary>
    public static event Action<TreeInstance> onTreeDestroyed;

    public static void invokeOnTreeDestroyed(TreeInstance treeInstance) {
        onTreeDestroyed?.Invoke(treeInstance);
    }

    /// <summary>
    /// This event is fired from triggers that open the inventory, it passes a boolean of whether the inventory should toggle
    /// </summary>
    public static event Action<bool> onPlayerToggleInventory;

    public static void invokeOnToggleInventory(bool shouldOpen) {
        onPlayerToggleInventory?.Invoke(shouldOpen);
    }

    /// <summary>
    /// This event is fired when a game object with the draggable component is hovered
    /// </summary>
    public static event Action onDraggableHovered;

    public static void invokeOnDraggableHovered() {
        onDraggableHovered?.Invoke();
    }

    /// <summary>
    /// This event is fired when a game object with the draggable component is unhovered
    /// </summary>
    public static event Action onDraggableUnHovered;

    public static void invokeOnDraggableUnHovered() {
        onDraggableUnHovered?.Invoke();
    }

    /// <summary>
    /// This event is fired when the mouse active state changes, it passes a bool describing if the mouse is active
    /// </summary>
    public static event Action<bool> onToggleMouseActive;

    public static void invokeToggleMouseActive(bool isMouseActive) {
        onToggleMouseActive?.Invoke(isMouseActive);
    }

    /// <summary>
    /// This event is fired from triggers that open the crafting ui, it passes a boolean of whether the crafting ui should toggle
    /// </summary>
    public static event Action<bool> onPlayerToggleCraftingUI;

    public static void invokeOnPlayerToggleCraftingUI(bool shouldOpen) {
        onPlayerToggleCraftingUI?.Invoke(shouldOpen);
    }

    /// <summary>
    /// This events is fired when the player clicks the inventory icon
    /// </summary>
    public static event Action onPlayerClickedToggleInventoryButton;

    public static void invokeOnPlayerClickedToggleInventoryButton() {
        onPlayerClickedToggleInventoryButton?.Invoke();
    }

    /// <summary>
    /// This events is fired when the player clicks the crafting icon
    /// </summary>
    public static event Action onPlayerClickedToggleCraftingButton;

    public static void invokeOnPlayerClickedToggleCraftingButton() {
        onPlayerClickedToggleCraftingButton?.Invoke();
    }

    /// <summary>
    /// This event is fired when the player crafts an item, it passes the item crafted
    /// </summary>
    public static event Action<Item> onPlayerCraftedItem;

    public static void invokeOnPlayerCraftedItem(Item item) {
        onPlayerCraftedItem?.Invoke(item);
    }
}