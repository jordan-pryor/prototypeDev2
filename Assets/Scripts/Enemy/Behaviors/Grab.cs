using UnityEngine;

[CreateAssetMenu(fileName = "Grab", menuName = "Enemy/Behaviors/Grab")]
public class Grab : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        //Supposed to be a dead by daylight inspired grab to escape from. not implemented currently
    }
}
