using UnityEngine;

[CreateAssetMenu(fileName = "MultipleBehavios", menuName = "Enemy/Behaviors/MultipleBehavios")]
public class MultipleBehavios : EnemyBehavior, IEnemy
{

    [SerializeField] private EnemyBehavior[] behaviorArray;

    public override void Execute(EnemyController controller)
    {
        controller.currentBehavior.Equals(behaviorArray[Random.Range(0, behaviorArray.Length)]);
    }
}
