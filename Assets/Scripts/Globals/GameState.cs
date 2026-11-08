/// <summary>
/// This class contains pieces of the games state, useful for variable you'd like to track globally
/// </summary>
public static class GameState {

    /// <summary>
    /// The item currently being dragged by the mouse
    /// </summary>
    public static Item DraggedItem;

    /// <summary>
    /// This indicates where a dragged item originated, used when swapping items
    /// </summary>
    public static DraggableItemsContainer DraggedItemOrigin;

    /// <summary>
    /// This is set true by a draggable item container, when an item being dragged is dropped onto it
    /// </summary>
    public static bool DraggedItemDroppedOnContainer;

    /// <summary>
    /// Is the mouse active
    /// </summary>
    public static bool IsMouseActive;

    /// <summary>
    /// Is the player's inventory open
    /// </summary>
    public static bool IsPlayerInventoryOpen;

    /// <summary>
    /// Is the player's crafting ui open
    /// </summary>
    public static bool IsCraftingUIOpen;

}