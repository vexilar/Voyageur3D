using UnityEngine;

public static class PlayerStats { // for now a static file will work, but eventually we will wants his stats to be in a save file

    public static readonly float CriticalMultiplier = 1.5f;

    public static int CalculatePlayerDamage(float damage, bool isCritical) {
        if (isCritical) {
            return Mathf.RoundToInt(damage * CriticalMultiplier);
        } else {
            return Mathf.RoundToInt(damage);
        }
    }

}