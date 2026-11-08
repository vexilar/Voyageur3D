using System.Collections;
using UnityEngine;

/// <summary>
/// Dev helper: grants crafting materials on Play so you can test the crafting UI.
/// Toggle via the Grant Starter Materials checkbox on this component (on Canvas/Inventory),
/// or disable the component entirely.
/// </summary>
public class DebugCraftingStarter : MonoBehaviour {

    [Tooltip("When enabled, grants a set of crafting materials when Play Mode starts.")]
    public bool GrantStarterMaterials = true;

    [Header("Amounts")]
    public int WoodenLogAmount = 100;
    public int IronOreAmount = 40;
    public int PlantFiberAmount = 20;
    public int StoneAmount = 15;
    public int BranchAmount = 15;

    IEnumerator Start() {
        if (!GrantStarterMaterials) {
            yield break;
        }

        // Wait one frame so ItemDefinitions can load into ItemAPI first.
        yield return null;

        Debug.Log("[DebugCraftingStarter] Granting starter crafting materials.");

        ItemAcquisition.AcquireItem(ItemSubType.WoodenLog, WoodenLogAmount);
        ItemAcquisition.AcquireItem(ItemSubType.IronOre, IronOreAmount);
        ItemAcquisition.AcquireItem(ItemSubType.PlantFiber, PlantFiberAmount);
        ItemAcquisition.AcquireItem(ItemSubType.Stone, StoneAmount);
        ItemAcquisition.AcquireItem(ItemSubType.Branch, BranchAmount);
    }

}
