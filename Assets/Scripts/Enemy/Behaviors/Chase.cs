using UnityEngine;

[CreateAssetMenu(fileName = "Chase", menuName = "Enemy/Behaviors/Chase")]
public class Chase : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
    
        Transform target = GameManager.instance.player.transform;
        if (controller.seenPlayer)
        {
            controller.agent.SetDestination(target.position);
            float distance = Vector3.Distance(controller.transform.position, target.position);
            controller.animator.SetBool("isWalking", true);
            if (distance <= controller.agent.stoppingDistance + 0.1f)
            {
                controller.animator.SetBool("isWalking", false);
                controller.SetBehavior(EnemyController.Behavior.Action);
            }
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
