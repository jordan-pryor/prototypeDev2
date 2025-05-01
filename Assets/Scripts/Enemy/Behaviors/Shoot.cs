using UnityEngine;

[CreateAssetMenu(fileName = "Shoot", menuName = "Enemy/Behaviors/Shoot")]
public class Shoot : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        controller.agent.isStopped = true;
        controller.animator.SetBool("isWalking", false);
        controller.animator.SetTrigger("isActing");
    }
}
