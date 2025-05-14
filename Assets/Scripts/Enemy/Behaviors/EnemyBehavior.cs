using UnityEngine;
public abstract class EnemyBehavior : ScriptableObject
{
    // Method to be implemented by all enemy behavior scripts
    public abstract void Execute(EnemyController controller);
}