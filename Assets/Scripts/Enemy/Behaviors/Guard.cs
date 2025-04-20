using UnityEngine;

[CreateAssetMenu(fileName = "Guard", menuName = "Enemy/Behaviors/Guard")]
public class Guard : EnemyBehavior
{
    public override void Execute(EnemyController controller)
    {
        if (controller.defaultPoints.Length == 0)
        {
            return;
        }
    }
}
