using UnityEngine;

[CreateAssetMenu(fileName = "Grab", menuName = "Enemy/Behaviors/Grab")]
public class Grab : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        //not entirely sure what grab is supposed to do but I'll ask later and presumably it's a melee attack
    }
}
