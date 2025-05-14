using UnityEngine;

[CreateAssetMenu(fileName = "Guard", menuName = "Enemy/Behaviors/Guard")]
public class Guard : EnemyBehavior, IEnemy
{
    // Executes guard behavior: stand at or return to a fixed point
    public override void Execute(EnemyController controller)
    {
        Transform targetPoint = GetGuardPoint(controller);                             // Get guard location
        float distance = Vector3.Distance(controller.transform.position, targetPoint.position);

        if (distance > controller.guardDistance)
        {
            // Move back to guard point
            controller.agent.isStopped = false;
            controller.agent.SetDestination(targetPoint.position);
            controller.animator.SetBool("isWalking", true);
        }
        else
        {
            // Remain stationary at guard point
            controller.agent.isStopped = true;
            controller.animator.SetBool("isWalking", false);
        }
    }

    // Returns guard point; generates one if missing
    public Transform GetGuardPoint(EnemyController controller)
    {
        if (controller.defaultPoints.Count > 0 && controller.defaultPoints[0] != null)
        {
            return controller.defaultPoints[0];
        }
        NewGuard(controller);
        return controller.defaultPoints[0];
    }

    // Creates or replaces the guard point by casting toward a wall
    public void NewGuard(EnemyController controller)
    {
        if (controller.defaultPoints.Count == 0)
            controller.defaultPoints.Add(controller.TowardWall(0.1f, 0.9f));
        else
            controller.defaultPoints[0] = controller.TowardWall(0.1f, 0.9f);
    }
}

