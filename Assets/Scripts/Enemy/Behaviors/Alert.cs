using UnityEngine;

[CreateAssetMenu(fileName = "Alert", menuName = "Enemy/Behaviors/Alert")]
public class Alert : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        controller.agent.isStopped = true;
        controller.animator.SetBool("isWalking", false);
        controller.animator.SetTrigger("isActing");
    }
}
