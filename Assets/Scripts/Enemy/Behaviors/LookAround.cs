using UnityEngine;

[CreateAssetMenu(fileName = "LookAround", menuName = "Enemy/Behaviors/LookAround")]
public class LookAround : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        controller.agent.SetDestination(controller.transform.position);
        controller.animator.SetBool("isSearching", true);
    }
}
