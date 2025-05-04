using UnityEngine;
public interface IEnemy
{
    // Executes the behavior logic using a reference to the enemy controller
    void Execute(EnemyController controller);
}
