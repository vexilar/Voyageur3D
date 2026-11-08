using UnityEngine;

public class PlayerHarvestingToolAnimationController : MonoBehaviour {

    PlayerHarvestingTool playerHarvestingTool;
    Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
        playerHarvestingTool = GetComponent<PlayerHarvestingTool>();
        playerHarvestingTool.onSwung += onAxeSwung;
        playerHarvestingTool.onMiss += onAxeMiss;
        playerHarvestingTool.onHit += onAxeHit;
    }

    private void onAxeHit() {
        animator.SetTrigger(WoodenPickaxeAnimationStates.Hit);
    }

    private void onAxeMiss() {
        animator.SetTrigger(WoodenPickaxeAnimationStates.Miss);
    }

    void onAxeSwung() {
        animator.SetTrigger(WoodenPickaxeAnimationStates.Swung);
    }

    private void OnDisable() {
        animator.ResetTrigger(WoodenPickaxeAnimationStates.Hit);
        animator.ResetTrigger(WoodenPickaxeAnimationStates.Miss);
        animator.ResetTrigger(WoodenPickaxeAnimationStates.Swung);
    }

}

public static class WoodenPickaxeAnimationStates {
    public static string Swung = "Swung";
    public static string Miss = "Miss";
    public static string Hit = "Hit";
}
