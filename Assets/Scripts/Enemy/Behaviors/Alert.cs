using UnityEngine;

[CreateAssetMenu(fileName = "Alert", menuName = "Enemy/Behaviors/Alert")]
public class Alert : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        controller.agent.SetDestination(controller.transform.position);
        controller.animator.SetBool("isActing", true);
        
    }
}
