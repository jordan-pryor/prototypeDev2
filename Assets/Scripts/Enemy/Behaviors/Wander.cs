using UnityEngine;

[CreateAssetMenu(fileName = "Wander", menuName = "Enemy/Behaviors/Wander")]
public class Wander : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        //choses random points around a certain position and wanders between them, similar to guard but with randomized locations
    }
}

