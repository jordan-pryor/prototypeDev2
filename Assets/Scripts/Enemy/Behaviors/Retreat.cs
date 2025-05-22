using UnityEngine;

[CreateAssetMenu(fileName = "Retreat", menuName = "Enemy/Behaviors/Retreat")]
public class Retreat : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        Vector3 playerDirection = GameManager.instance.player.transform.position - controller.transform.position;
        if (controller.seenPlayer)
        {
            controller.agent.SetDestination(controller.transform.position + (10 * playerDirection));
            controller.animator.SetBool("isWalking", true);
        }
        else
        {
            if (controller.memoryTimer <= 0f)
            {
                controller.animator.SetBool("isWalking", false);
                controller.SetBehavior(EnemyController.Behavior.Search);
            }
        }
    }
}
