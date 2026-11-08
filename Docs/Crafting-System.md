# Crafting System

How player crafting works in Voyageur: data definitions, UI flow, and inventory side effects.

## Overview

Crafting is data-driven and event-based. Recipes live on `Item` definitions. The crafting UI lists every craftable item grouped by `ItemType`. Clicking a craftable item checks inventory requirements, then fires an event that adds the result and consumes ingredients.

There is no separate recipe database. An item is craftable when `IsCraftable` is true and its `CraftingRecipe` list is filled in on the **ItemDefinitions** prefab.

## Core Data

### Item (`Assets/Items/Scripts/Item.cs`)

Relevant fields:

| Field | Role |
| --- | --- |
| `IsCraftable` | Shown in the crafting UI when true |
| `CraftingRecipe` | List of `CraftingRequirement` ingredients |
| `ItemType` | Groups items in the UI (`Empty`, `Placable`, `Equippable`) |
| `ItemSubType` | Unique item identity used for lookups and inventory checks |

### CraftingRequirement (`Assets/Items/Scripts/CraftingRequirement.cs`)

```csharp
public class CraftingRequirement {
    public ItemSubType ItemSubType;
    public int Amount;
}
```

### Item definitions source of truth

`ItemDefinitions` (`Assets/Items/Scripts/ItemDefinitions.cs`) holds the full item list in the Inspector and loads it into `ItemAPI` on `Start`:

```csharp
ItemAPI.LoadItemsFromEditor(Items);
```

Edit recipes on the prefab at:

`Assets/Items/Prefabs/ItemDefinitions.prefab`

Do not edit a scene copy of this object; `ItemAPI` expects the prefab definitions.

### ItemAPI helpers used by crafting

- `GetCraftableItems(ItemType)` — items of that type with `IsCraftable == true`
- `GetItem(ItemSubType)` — lookup for icons / names in tooltips and components
- `CreateItem(ItemSubType, amount)` — clones definition data into a new inventory instance (copies recipe and craftable flags)

## Opening and Closing the UI

| Input | Behavior |
| --- | --- |
| `C` | Toggle crafting UI (`UIStateManager`) |
| Crafting button click | Same toggle via `CraftingButton` → `Events.invokeOnPlayerClickedToggleCraftingButton()` |
| `I` | Opens inventory and closes crafting |
| `Escape` | Closes crafting and inventory |

`UIStateManager.toggleCraftingUI()`:

1. Closes inventory
2. Toggles mouse cursor active state
3. Invokes `Events.invokeOnPlayerToggleCraftingUI(...)`

`PlayerCraftingUI` listens for that event, sets `GameState.IsCraftingUIOpen`, and shows/hides its `ToggleContainer`.

## UI Hierarchy

```
PlayerCraftingUI
└── CraftablesContainer
    └── CraftingGroup (one per ItemType)
        └── CraftableItem (one per craftable item of that type)
```

Prefabs under `Assets/UIElements/`:

- `Crafting Group.prefab`
- `Craftable Item.prefab`
- `Crafting Component.prefab` (ingredient row inside the tooltip)

### PlayerCraftingUI

On start, instantiates one `CraftingGroup` per `ItemType` enum value and assigns that type to the group.

### CraftingGroup

On enable (and after a successful craft):

1. Clears previous craftable item children
2. Sets the group title to the `ItemType` name
3. Asks `ItemAPI.GetCraftableItems(ItemType)`
4. Instantiates a `CraftableItem` per result and wires its `Item` + tooltip data

### CraftableItem

- Shows the item sprite
- Tints with `CantCraftColor` when requirements are unmet
- On click, if all requirements are met, calls `Events.invokeOnPlayerCraftedItem(Item)`

Requirement check:

```csharp
PlayerItems.InventoryContains(requirement.ItemSubType, requirement.Amount)
```

for every entry in `Item.CraftingRecipe`.

### Tooltip: CraftingItemTooltip

Hovering a craftable item opens a crafting tooltip (`TooltipType.CraftingItem`) that shows:

- Name, description, icon
- One `CraftingComponent` row per recipe ingredient (icon + required amount)

`CraftingComponent` greys out the ingredient when the player does not have enough.

## Craft Execution Flow

```
CraftableItem.onPlayerClicked()
  → canCraft()?
  → Events.invokeOnPlayerCraftedItem(item)
      ├─ PlayerInventory.onPlayerCraftedItem
      │    1. ItemAcquisition.AcquireItem(item.ItemSubType, 1)
      │    2. For each recipe component: consumeItem(subType, amount)
      └─ CraftingGroup.onPlayerCraftedItem
           → re-render group (refresh craftable / unavailable colors)
```

### Inventory effects (`PlayerInventory`)

1. **Grant result** — `ItemAcquisition.AcquireItem` creates the item via `ItemAPI`, shows system text (“You acquired…”), then fires `onItemAcquired`, which stacks or adds into the inventory list.
2. **Consume ingredients** — finds the first inventory stack matching each requirement’s `ItemSubType`, subtracts `Amount`, and removes the stack if it hits zero.

Inventory contents are mirrored to `PlayerItems.InventoryItems` whenever the draggable inventory changes, so craftability checks stay in sync with what the player holds.

## Current Craftable Recipes

From `ItemDefinitions.prefab` (as of this doc):

| Result | Type | Recipe |
| --- | --- | --- |
| Wooden Axe | Equippable | 15 Wooden Log, 10 Iron Ore |
| Wooden Bow | Equippable | 10 Wooden Log, 15 Plant Fiber |
| Wooden Pickaxe | Equippable | 10 Wooden Log, 25 Iron Ore |
| Stone Axe | Equippable | 10 Stone, 10 Branch |
| Wooden Platform | Placable | 50 Wooden Log |

Other placables (Stone Wall, Stone Doorway, Wooden Door, Wall Torch, Wooden Chest) exist as items but have `IsCraftable: false` and empty recipes.

Raw materials used as ingredients (Wooden Log, Stone, Plant Fiber, Branch, Iron Ore) are not craftable.

## Key Scripts

| Script | Path | Role |
| --- | --- | --- |
| `Item` | `Assets/Items/Scripts/Item.cs` | Item schema including recipe |
| `CraftingRequirement` | `Assets/Items/Scripts/CraftingRequirement.cs` | Ingredient entry |
| `ItemDefinitions` | `Assets/Items/Scripts/ItemDefinitions.cs` | Loads item list into `ItemAPI` |
| `ItemAPI` | `Assets/Items/Scripts/ItemApi.cs` | Craftable queries / item creation |
| `PlayerCraftingUI` | `Assets/Scripts/PlayerUI/PlayerCraftingUI.cs` | Crafting panel + group spawn |
| `CraftingGroup` | `Assets/Scripts/PlayerUI/CraftingGroup.cs` | Per-type craftable list |
| `CraftableItem` | `Assets/Scripts/PlayerUI/CraftableItem.cs` | Click-to-craft + affordance tint |
| `CraftingComponent` | `Assets/Scripts/PlayerUI/CraftingComponent.cs` | Ingredient row in tooltip |
| `CraftingItemTooltip` | `Assets/Scripts/PlayerUI/CraftingItemTooltip.cs` | Crafting tooltip contents |
| `CraftingButton` | `Assets/Scripts/PlayerUI/CraftingButton.cs` | UI button toggle |
| `UIStateManager` | `Assets/Scripts/PlayerUI/UIStateManager.cs` | Keyboard / button UI state |
| `PlayerInventory` | `Assets/Scripts/PlayerUI/PlayerInventory.cs` | Grant result + consume recipe |
| `PlayerItems` | `Assets/Scripts/Player/PlayerItems.cs` | Global inventory query helper |
| `ItemAcquisition` | `Assets/Scripts/PlayerUI/ItemAcquisition.cs` | Create item + notify inventory |
| `Events` | `Assets/Scripts/Globals/Events.cs` | Crafting UI + craft success events |
| `GameState` | `Assets/Scripts/Globals/GameState.cs` | `IsCraftingUIOpen` flag |

## Adding a New Craftable Item

1. Add a new `ItemSubType` value at the **end** of the enum (`Assets/Items/Scripts/ItemSubType.cs`).
2. On `ItemDefinitions.prefab`, add an `Item` with:
   - `IsCraftable = true`
   - `ItemType` set to the group it should appear under
   - `CraftingRecipe` filled with ingredient subtypes and amounts
   - Sprite / name / description as needed
3. No UI code changes are required; groups refresh from `ItemAPI.GetCraftableItems`.

## Testing: starter materials

`DebugCraftingStarter` on `Canvas/Inventory` grants crafting materials when Play Mode starts.

**Toggle it in the Inspector:**

1. Select `Canvas/Inventory` in the Hierarchy
2. Find the **Debug Crafting Starter** component
3. Uncheck **Grant Starter Materials** to stop auto-granting (or disable the component)

Default amounts are enough to craft every current recipe at least once (logs, iron ore, plant fiber, stone, branches).

## Notes / Caveats

- Crafting is instantaneous; there is no progress bar, station, or crafting queue.
- Ingredient consumption looks up the first matching stack only (does not split across multiple stacks beyond summing for the `canCraft` check via `Sum` in `PlayerItems.InventoryContains`).
- `ItemAPI` is loaded in `ItemDefinitions.Start`. Script execution order is relied on so definitions load before most consumers.
- Opening crafting closes inventory (and vice versa).
