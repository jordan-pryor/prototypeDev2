using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Wander", menuName = "Enemy/Behaviors/Wander")]
public class Wander : EnemyBehavior, IEnemy
{
    public override void Execute(EnemyController controller)
    {
        //choses random points around a certain position and wanders between them, similar to guard but with randomized locations
        Vector3 ranPosition = Random.insideUnitSphere * controller.guardDistance;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPosition, out hit, controller.guardDistance, 1);
        controller.agent.SetDestination(hit.position);
    }
}

