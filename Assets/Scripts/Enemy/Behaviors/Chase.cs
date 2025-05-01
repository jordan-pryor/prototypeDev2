using UnityEngine;

[CreateAssetMenu(fileName = "Chase", menuName = "Enemy/Behaviors/Chase")]
public class Chase : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        Debug.Log("Move");
        if (!controller.seenPlayer)
        {
            if (controller.memoryTimer <= 0f)
            {
                controller.agent.isStopped = true;
                controller.agent.ResetPath();
                controller.animator.SetBool("isWalking", false);
                controller.SetBehavior(EnemyController.Behavior.Search);
            }
            return;
        }

        Transform target = GameManager.instance.player.transform;
        controller.agent.stoppingDistance = controller.actionRange;
        float distance = Vector3.Distance(controller.transform.position, target.position);

        if (distance > controller.actionRange )
        {
            controller.agent.isStopped = false;
            controller.agent.SetDestination(target.position);
            controller.animator.SetBool("isWalking", true);
        }
        else
        {
            controller.agent.isStopped = true;
            controller.agent.ResetPath();
            controller.animator.SetBool("isWalking", false);
            if (GameManager.instance.playerController.currentStealth < controller.detection)
            {
                controller.SetBehavior(EnemyController.Behavior.Action);
            }
            else
            {
                controller.SetBehavior(EnemyController.Behavior.Search);
            }
        }
    }
}