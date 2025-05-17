using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Slash", menuName = "Enemy/Behaviors/Slash")]
public class Slash : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        //instantiate a trigger in front of the enemy that only lasts for a split second, that does damage on contact, and then is removed

    }
}
