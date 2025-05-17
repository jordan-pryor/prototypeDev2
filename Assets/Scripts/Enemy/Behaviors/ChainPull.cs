using UnityEngine;

[CreateAssetMenu(fileName = "ChainPull", menuName = "Enemy/Behaviors/ChainPull")]
public class ChainPull : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        Vector3 playerDirection = GameManager.instance.player.transform.position - controller.transform.position;
        
        //instatiate a trigger projectile that travels the distance of chain range

        //if the trigger comes into contact with the player, start moving the player towards the knight
    }
}
