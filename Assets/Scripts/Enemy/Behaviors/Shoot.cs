using UnityEngine;

[CreateAssetMenu(fileName = "Shoot", menuName = "Enemy/Behaviors/Shoot")]
public class Shoot : EnemyBehavior, IEnemy
{
    // Executes the shooting behavior for an enemy
    public override void Execute(EnemyController controller)
    {
        controller.agent.isStopped = true;                      // Stop enemy movement
        controller.animator.SetBool("isWalking", false);        // Ensure walk animation is off
        controller.animator.SetTrigger("isActing");             // Trigger the shoot animation
    }
}
