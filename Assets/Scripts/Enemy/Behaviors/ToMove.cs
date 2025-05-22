using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "ToMove", menuName = "Enemy/Behaviors/ToMove")]
public class ToMove : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        if (controller.currentBehavior == EnemyController.Behavior.Default)
        {
            controller.SetBehavior(EnemyController.Behavior.Move, GameManager.instance.playerController.transform);
        }
        else {
            controller.SetBehavior(EnemyController.Behavior.Action);
        }
    }
}