using System.Collections;
using UnityEngine;

public class PlayerBowFireArrow : MonoBehaviour {

    [Tooltip("A GameObject that represents  the spawn location when firing an arrow from the bow")]
    public GameObject ArrowSpawnLocation;

    [Tooltip("The base force applied to the arrow when firing")]
    public float BowTension = 8f;

    [Tooltip("If this is true, the arrow will fly longer if the player fully draws the bow, and shorter if he only partially draws it")]
    public bool DrawDurationAffectsArrowSpeed = true;

    [Tooltip("If this is true, the arrow will do more damage the longer the player draws the bow")]
    public bool DrawDurationAffectsArrowDamage = true;

    [Tooltip("If draw affects arrow speed is true, this sets the maximum multipier of force applied to the arrow if the player has drawn the bow long enough")]
    public float MaxDrawForceMultiplier = 1f;

    [Tooltip("If draw affects arrow damage is true, this sets the maximum multipier of damage applied to the arrow if the player has drawn the bow long enough")]
    public float MaxDrawDamageMultipier = 1f;

    [Tooltip("If assigned, the loosed arrow will copy the rotation of this object's transform")]
    public Transform FiredArrowCopyDirection;

    [Range(.1f, 5f)]
    [Tooltip("A multiplier applied to how quickly the bow's draw duration increases, applies to damage if DrawDurationAffectsArrowDamage is true, and applies to force" +
        "if DrawDurationAffectsArrowSpeed is true")]
    public float DrawDurationRatio = 2f;

    public GameObject WoodenArrow;

    PlayerBow playerBow;

    private void Start() {
        playerBow = GetComponent<PlayerBow>();

        playerBow.onPlayerRelease += shootArrow;
    }

    void shootArrow(float drawDuration, int bowDamage) {
        StartCoroutine(shootAfterDelay(drawDuration, bowDamage));
    }

    void looseArrow(float drawDuration, int bowDamage) {
        var loosedArrow = Instantiate(WoodenArrow, ArrowSpawnLocation.transform.position, Quaternion.identity);
        applyDamageToArrow(loosedArrow, drawDuration, bowDamage);

        if (FiredArrowCopyDirection) {
            loosedArrow.transform.forward = FiredArrowCopyDirection.forward;
        }

        var rigid = loosedArrow.GetComponent<Rigidbody>();

        if (rigid) {
            var direction = transform.forward;
            var force = BowTension;

            if (DrawDurationAffectsArrowSpeed) {
                var drawDurationMultiplier = drawDuration * DrawDurationRatio;
                drawDurationMultiplier = Mathf.Clamp(drawDurationMultiplier, 0, MaxDrawForceMultiplier);

                force *= drawDurationMultiplier;
            }

            var finalForce = direction * force;
            rigid.AddForce(finalForce, ForceMode.Impulse);
        }
    }

    void applyDamageToArrow(GameObject arrow, float drawDuration, int bowDamage) {
        var damageSource = arrow.GetComponent<DamageSource>();

        if (damageSource) {
            if (DrawDurationAffectsArrowDamage) {
                var drawDurationMultiplier = drawDuration * DrawDurationRatio;
                drawDurationMultiplier = Mathf.Clamp(drawDurationMultiplier, 0, MaxDrawDamageMultipier);
                bowDamage *= Mathf.RoundToInt(drawDurationMultiplier);
            }

            damageSource.Damage += bowDamage; // we add here because the damagesource (the arrow) already has it's own damage we want to preserve
        }
    }

    IEnumerator shootAfterDelay(float drawDuration, int bowDamage) {
        yield return new WaitForSeconds(.05f);

        looseArrow(drawDuration, bowDamage);
    }
}
