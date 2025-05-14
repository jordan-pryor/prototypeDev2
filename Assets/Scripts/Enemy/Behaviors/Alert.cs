using UnityEngine;

[CreateAssetMenu(fileName = "Alert", menuName = "Enemy/Behaviors/Alert")]
public class Alert : EnemyBehavior, IEnemy
{
    // Executes the alert behavior (non-combat, reactive animation)
    public override void Execute(EnemyController controller)
    {
        controller.agent.isStopped = true;                   // Stop movement
        controller.animator.SetBool("isWalking", false);     // Ensure walking animation is off
        controller.animator.SetTrigger("isActing");          // Trigger alert animation (e.g. looking up or reacting)
    }
}
