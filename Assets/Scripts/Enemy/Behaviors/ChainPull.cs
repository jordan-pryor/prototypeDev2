using UnityEngine;

[CreateAssetMenu(fileName = "ChainPull", menuName = "Enemy/Behaviors/ChainPull")]
public class ChainPull : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        Transform playerPos = GameManager.instance.player.transform;
        if (Vector3.Distance(controller.transform.position, playerPos.position) > controller.actionRange)
        {
            controller.SetBehavior(EnemyController.Behavior.Search);
        }
        else
        {
            controller.Fire();
        }

    }
}
