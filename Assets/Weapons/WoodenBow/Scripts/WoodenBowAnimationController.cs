using UnityEngine;

public class WoodenBowAnimationController : MonoBehaviour {

    PlayerBow playerBow;
    Animator animator;

    private void Start() {
        playerBow = GetComponent<PlayerBow>();
        animator = GetComponent<Animator>();

        playerBow.onPlayerKnockArrow += onPlayerKnockArrow;
        playerBow.onPlayerRelease += onPlayerReleaseBow;
        playerBow.onPlayerDrawBow += onPlayerDrawBow;
        playerBow.onCancelKnockArrow += onCancelKnockArrow;
        playerBow.onCancelDrawBow += onCancelDrawBow;
    }

    void onCancelKnockArrow() {
        animator.ResetTrigger(WoodenBowAnimations.KnockArrow);
    }

    void onCancelDrawBow() {
        animator.SetBool(WoodenBowAnimations.DrawBow, false);
    }

    void onPlayerKnockArrow() {
        animator.SetTrigger(WoodenBowAnimations.KnockArrow);
    }

    void onPlayerDrawBow() {
        animator.SetBool(WoodenBowAnimations.DrawBow, true);
    }

    void onPlayerReleaseBow(float drawDuration, int bowDamage) {
        animator.SetBool(WoodenBowAnimations.DrawBow, false);
    }

}

public static class WoodenBowAnimations {
    public static string KnockArrow = "KnockArrow";
    public static string DrawBow = "DrawBow";
}
