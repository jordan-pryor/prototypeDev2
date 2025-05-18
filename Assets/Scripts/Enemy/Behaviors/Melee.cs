using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee", menuName = "Enemy/Behaviors/Melee")]
public class Melee : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        controller.Melee();
        Transform playerPos = GameManager.instance.player.transform;
        if (Vector3.Distance(controller.transform.position, playerPos.position) > controller.actionRange)
        {
            controller.SetBehavior(EnemyController.Behavior.Search);
        }
    }
}
