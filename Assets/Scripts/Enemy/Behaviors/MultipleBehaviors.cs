using UnityEngine;

[CreateAssetMenu(fileName = "MultipleBehaviors", menuName = "Enemy/Behaviors/MultipleBehaviors")]
public class MultipleBehaviors : EnemyBehavior, IEnemy
{

    [SerializeField] private EnemyBehavior[] behaviorArray;

    public override void Execute(EnemyController controller)
    {
        controller.currentBehavior.Equals(behaviorArray[Random.Range(0, behaviorArray.Length)]);
    }
}
