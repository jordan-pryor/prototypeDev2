using UnityEngine;

[CreateAssetMenu(fileName = "ChainPull", menuName = "Enemy/Behaviors/ChainPull")]
public class ChainPull : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        controller.agent.isStopped = true;                   // Stop enemy movement
        controller.animator.SetBool("isWalking", false);     // Ensure walk animation is off
        controller.animator.SetTrigger("isSecondary");          // Trigger attack animation
        Transform playerPos = GameManager.instance.player.transform;
        if (Vector3.Distance(controller.transform.position, playerPos.position) > controller.actionRange)
        {
            controller.SetBehavior(EnemyController.Behavior.Search);
        }
        else
        {
            controller.Fire();
        }

    }
}
