using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Wander", menuName = "Enemy/Behaviors/Wander")]
public class Wander : EnemyBehavior, IEnemy
{
    Transform wanderPoint;   // Current wander target
    float timer;         // Wait timer at target
    public override void Execute(EnemyController con)
    {
        if (wanderPoint == null) wanderPoint = con.TowardWall(0.4f, 0.6f);
        float dis = Vector3.Distance(con.transform.position, wanderPoint.position);
        if (dis > con.guardDistance)
        {
            timer = con.patrolWait;
            con.agent.isStopped = false;
            con.agent.SetDestination(wanderPoint.position);
            con.animator.SetBool("isWalking", true);
        }
        else
        {
            con.agent.isStopped = true;
            con.animator.SetBool("isWalking", false);
            timer -= Time.deltaTime;
            if (timer > 0f) return;
            // Destroy old target
            if (wanderPoint != null) Object.Destroy(wanderPoint.gameObject);
            wanderPoint = con.TowardWall(0.4f, 0.6f);   // New random point
            timer = con.patrolWait;
            con.agent.isStopped = false;
            con.agent.SetDestination(wanderPoint.position);
            con.animator.SetBool("isWalking", true);
        }
    }
}