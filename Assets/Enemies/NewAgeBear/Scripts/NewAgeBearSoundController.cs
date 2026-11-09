using UnityEngine;

public class NewAgeBearSoundController : MonoBehaviour {
    public AudioClip enemyAttention;
    public AudioClip enemyRoar;
    public AudioClip enemyChase;
    public AudioClip enemyMeleeAttack;
    public AudioClip enemyLostTarget;
    public AudioClip enemyHurt;
    public AudioClip enemyDeath;

    AudioSource audioSource;
    Enemy enemy;
    EnemyPursue enemyPursue;
    EnemyMeleeAttack enemyMelee;
    EnemyHealth enemyHealth;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        enemy = GetComponent<Enemy>();
        enemyPursue = GetComponent<EnemyPursue>();
        enemyMelee = GetComponent<EnemyMeleeAttack>();
        enemyHealth = GetComponent<EnemyHealth>();

        enemy.onHeardTarget += onHeardPlayer;
        enemy.onSeenTarget += onSeenPlayer;
        enemy.onNoLongerSuspicious += onLostTarget;
        enemyMelee.onMeleeAttack += onMeleeAttack;
        enemyPursue.onPursue += onPursue;
        enemyPursue.onTargetAbandoned += onLostTarget;
        enemyHealth.onEnemyDied += onEnemyDied;
        enemyHealth.onEnemyHurt += onEnemyHurt;
    }

    void onEnemyDied() {
        audioSource.PlayAudio(enemyDeath);
    }

    void onEnemyHurt() {
        audioSource.PlayAudio(enemyHurt);
    }

    void onLostTarget() {
        audioSource.PlayAudio(enemyLostTarget);
    }

    void onMeleeAttack() {
        audioSource.PlayAudio(enemyMeleeAttack, false, .3f);
    }

    void onPursue() {
        audioSource.PlayAudio(enemyChase);
    }

    void onSeenPlayer(GameObject target) {
        audioSource.PlayAudio(enemyRoar);
    }

    void onHeardPlayer(GameObject target) {
        audioSource.PlayAudio(enemyAttention);
    }

    /// <summary>
    /// Client-side FX playback driven by <see cref="Voyageur.Networking.NetworkEnemyAuthority"/>.
    /// </summary>
    public void PlayNetworkFx(string fx) {
        if (audioSource == null) {
            audioSource = GetComponent<AudioSource>();
        }

        switch (fx) {
            case "Attention":
                audioSource.PlayAudio(enemyAttention);
                break;
            case "Roar":
                audioSource.PlayAudio(enemyRoar);
                break;
            case "Pursue":
                audioSource.PlayAudio(enemyChase);
                break;
            case "MeleeAttack":
                audioSource.PlayAudio(enemyMeleeAttack, false, .3f);
                break;
            case "LostTarget":
                audioSource.PlayAudio(enemyLostTarget);
                break;
            case "Hurt":
                audioSource.PlayAudio(enemyHurt);
                break;
            case "Death":
                audioSource.PlayAudio(enemyDeath);
                break;
        }
    }

}
