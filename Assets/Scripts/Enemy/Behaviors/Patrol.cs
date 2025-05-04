using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Patrol", menuName = "Enemy/Behaviors/Patrol")]
public class Patrol : EnemyBehavior, IEnemy
{
    float timer;
    // Main execution logic for patrol behavior
    public override void Execute(EnemyController con)
    {
        Transform targetPoint = GetPatrolPoint(con);                                // Get the current patrol target
        float dis = Vector3.Distance(con.transform.position, targetPoint.position); // Distance to target

        if (dis > con.guardDistance)
        {
            // Move toward patrol point
            timer = con.patrolWait;
            con.agent.isStopped = false;
            con.agent.SetDestination(targetPoint.position);
            con.animator.SetBool("isWalking", true);
        }
        else
        {
            // Wait at patrol point, then advance to next
            con.agent.isStopped = true;
            con.animator.SetBool("isWalking", false);
            timer -= Time.deltaTime;
            if (timer > 0f) return;

            Advance(con); // Move to next patrol index
            timer = con.patrolWait;
            con.agent.isStopped = false;
            con.agent.SetDestination(GetPatrolPoint(con).position);
            con.animator.SetBool("isWalking", true);
        }
    }
    // Returns the patrol point at current index; generates if needed
    public Transform GetPatrolPoint(EnemyController con)
    {
        if (con.defaultPoints.Count == 0 || con.defaultPoints[0] == null || con.defaultPoints.Count < con.patrolPoints)
            NewPatrol(con);
        return con.defaultPoints[con.patrolIndex];
    }
    // Generates patrol points by casting toward walls
    public void NewPatrol(EnemyController con)
    {
        while (con.defaultPoints.Count < con.patrolPoints)
            con.defaultPoints.Add(con.TowardWall(0.4f, 0.6f));
    }
    // Advances patrol index based on patrol mode
    public void Advance(EnemyController con)
    {
        int count = con.defaultPoints.Count;
        if (count <= 1) return;

        switch (con.pathMode)
        {
            case EnemyController.PathMode.Cycle:
                con.patrolIndex = (con.patrolIndex + 1) % count; break;

            case EnemyController.PathMode.PingPong:
                con.patrolIndex += con.patrolDir;
                if (con.patrolIndex >= count || con.patrolIndex < 0)
                {
                    con.patrolDir *= -1;
                    con.patrolIndex += 2 * con.patrolDir;
                }
                break;

            case EnemyController.PathMode.Random:
                int next;
                do { next = Random.Range(0, count); } while (next == con.patrolIndex);
                con.patrolIndex = next; break;
        }
    }
}
