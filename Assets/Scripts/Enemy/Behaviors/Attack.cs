using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Enemy/Behaviors/Attack")]
public class Attack : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        controller.agent.isStopped = true;
        controller.animator.SetBool("isWalking", false);
        controller.animator.SetTrigger("isActing");
    }
}
