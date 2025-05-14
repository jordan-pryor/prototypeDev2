using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Enemy/Behaviors/Attack")]
public class Attack : EnemyBehavior, IEnemy
{
    // Executes the attack behavior
    public override void Execute(EnemyController controller)
    {
        controller.agent.isStopped = true;                   // Stop enemy movement
        controller.animator.SetBool("isWalking", false);     // Ensure walk animation is off
        controller.animator.SetTrigger("isActing");          // Trigger attack animation
    }
}
