using UnityEngine;

[CreateAssetMenu(fileName = "Guard", menuName = "Enemy/Behaviors/Guard")]
public class Guard : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        if (controller.defaultPoints.Length == 0) return;
        Transform targetPoint = controller.defaultPoints[0];
        float distance = Vector3.Distance(controller.transform.position, targetPoint.position);
        if (distance > controller.guardDistance)
        {
            controller.agent.SetDestination(targetPoint.position);
            controller.animator.SetBool("isWalking", true);
        }
        else
        {
            controller.agent.SetDestination(controller.transform.position);
            controller.animator.SetBool("isWalking", false);
        }
    }
}
