using UnityEngine;
using UnityEngine.AI;

public class NewAgeBearAnimationController : MonoBehaviour {

    Animator animator;
    Enemy enemy;
    NavMeshAgent agent;
    EnemyMeleeAttack enemyMeleeAttack;
    EnemyTurnToInspect enemyTurnToInspect;
    EnemyPursue enemyPursue;
    EnemyHealth enemyHealth;

    public bool isDead;

    void Start() {
        enemy = GetComponent<Enemy>();
        enemyMeleeAttack = GetComponent<EnemyMeleeAttack>();
        enemyTurnToInspect = GetComponent<EnemyTurnToInspect>();
        enemyPursue = GetComponent<EnemyPursue>();
        enemyHealth = GetComponent<EnemyHealth>();

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        enemy.onHeardTarget += onHeardTarget;
        enemy.onNoLongerSuspicious += onLostTarget;
        enemy.onSeenTarget += onSeenTarget;
        enemyPursue.onTargetAbandoned += onLostTarget;
        enemyMeleeAttack.onMeleeAttack += onMeleeAttack;
        enemyHealth.onEnemyDied += onEnemyDied;
        enemyHealth.onEnemyHurt += onEnemyHurt;
    }

    void onEnemyHurt() {
        animator.SetTrigger(NewAgeBearAnimationStates.Hurt);
    }

    void onEnemyDied() {
        isDead = true;
    }

    void onMeleeAttack() {
        animator.SetTrigger(NewAgeBearAnimationStates.MeleeAttack);
    }

    void onSeenTarget(GameObject target) {
        animator.SetTrigger(NewAgeBearAnimationStates.Roar);
    }

    void onHeardTarget(GameObject target) {
        playAttentionAnimation();
    }

    void playAttentionAnimation() {
        animator.SetTrigger(NewAgeBearAnimationStates.Attention);
    }

    void onLostTarget() {
        playAttentionAnimation();
    }

    private void Update() {
        var horizontalMagnitude = new Vector2(agent.velocity.x, agent.velocity.z).magnitude;
        animator.SetFloat(NewAgeBearAnimationStates.Walking, horizontalMagnitude);
    }

}

public static class NewAgeBearAnimationStates {
    public static string Idle = "Idle";
    public static string Attention = "Attention";
    public static string Walking = "Walking";
    public static string Roar = "Roar";
    public static string TurnRight = "TurnRight";
    public static string TurnLeft = "TurnLeft";
    public static string MeleeAttack = "MeleeAttack";
    public static string Hurt = "Hurt";
}
