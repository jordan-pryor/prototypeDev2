using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee", menuName = "Enemy/Behaviors/Melee")]
public class Melee : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        //instantiate a trigger in front of the enemy that only lasts for a split second, that does damage on contact, and then is removed
        //basically the same as slash but the trigger is over the entire enemy model instead of a focused area
    }
}
