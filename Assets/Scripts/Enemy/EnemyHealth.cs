using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    public event Action onEnemyDied;
    public event Action onEnemyHurt;

    [Tooltip("The health pool of the enemy, when this reaches 0, he dies")]
    public int Health = 100;

    List<EnemyHitbox> enemyHitboxes = new List<EnemyHitbox>();

    private void Start() {
        enemyHitboxes = GetComponentsInChildren<EnemyHitbox>().ToList();
        enemyHitboxes.ForEach(enemyHitbox => enemyHitbox.onHit += onHit);
    }

    void onHit(EnemyBodyPartType enemyBodyPartType, bool isCritical, DamageSource source) {
        var damage = source.Damage;

        if (source.IsFromPlayer) {
            damage = PlayerStats.CalculatePlayerDamage(damage, isCritical);
        }

        Events.invokeOnEnemyHit(gameObject, source);

        Health -= damage;

        Debug.Log($"Ouch my health is {Health}");

        if (Health <= 0) {
            onEnemyDied?.Invoke();
        } else {
            onEnemyHurt?.Invoke();
        }
    }

}
