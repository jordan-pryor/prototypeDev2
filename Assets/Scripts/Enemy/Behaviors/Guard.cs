using UnityEngine;

[CreateAssetMenu(fileName = "Guard", menuName = "Enemy/Behaviors/Guard")]
public class Guard : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        Transform targetPoint = GetGuardPoint(controller);
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
    public Transform GetGuardPoint(EnemyController controller)
    {
        if(controller.defaultPoints.Count > 0 && controller.defaultPoints[0] != null)
        {
            return controller.defaultPoints[0];
        }
        NewGuard( controller);
        return controller.defaultPoints[0];
    }
    public void NewGuard(EnemyController controller)
    {
        if(controller.defaultPoints.Count == 0) controller.defaultPoints.Add(controller.transform);
        else controller.defaultPoints[0] = controller.transform;
    }
}
