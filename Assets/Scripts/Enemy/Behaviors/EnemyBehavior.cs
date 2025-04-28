using UnityEngine;
public abstract class EnemyBehavior : ScriptableObject
{
    public abstract void Execute(EnemyController controller);
}