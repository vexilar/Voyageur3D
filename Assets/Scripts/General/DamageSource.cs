using System;
using UnityEngine;
using UnityEngine.Events;

public class DamageSource : MonoBehaviour {

    public event Action onHit;

    [Tooltip("The damage associated with this projectile, for example, if it's an arrow, the currently equipped bow's damage would be applied to the projectile")]
    public int Damage = 10;

    [Tooltip("If the damage source is the player, damage is treated differently when it comes from the player, for example, critical multiplier is added if the source hits a critical area")]
    public bool IsFromPlayer = true;

    [Tooltip("when true, the damage source is disabled when it collides with a surface")]
    public bool IsProjectile;

    [Tooltip("Add events here to play when the damage source hits the target")]
    public UnityEvent playOnHit;

    [NonSerialized]
    public bool ShouldBeIgnored;

    private void OnCollisionEnter(Collision collision) {
        if (IsProjectile) {
            damageSourceHit();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (IsProjectile) {
            damageSourceHit();
        }
    }

    public void onHitCallBack() { // this is invoked by a receiver of the damage source, like an enemy hitbox being hit
        ShouldBeIgnored = true;
    }

    void damageSourceHit() {
        playOnHit?.Invoke();
        onHit?.Invoke();
    }

}
