using UnityEngine;

[CreateAssetMenu(fileName = "Chase", menuName = "Enemy/Behaviors/Chase")]
public class Chase : EnemyBehavior, IEnemy
{
        public override void Execute(EnemyController controller)
        {
            Transform target = null;
            // Decide target based on sight or sound
            if (controller.seenPlayer)
            {
                target = GameManager.instance.player.transform;
            }
            else if (controller.targetPoints.Count > 0)
            {
                // Use last known sound location
                target = controller.targetPoints[0];
            }
            if (target == null)
            {
                if (controller.memoryTimer <= 0f)
                {
                    controller.agent.isStopped = true;
                    controller.agent.ResetPath();
                    controller.animator.SetBool("isWalking", false);
                    controller.SetBehavior(EnemyController.Behavior.Search);
                }
                return;
            }

            float distance = Vector3.Distance(controller.transform.position, target.position);
            controller.agent.stoppingDistance = controller.actionRange;

            if (distance > controller.actionRange)
            {
                // Move toward target
                controller.agent.isStopped = false;
                controller.agent.SetDestination(target.position);
                controller.animator.SetBool("isWalking", true);
            }
            else
            {
                // At target location
                controller.agent.isStopped = true;
                controller.agent.ResetPath();
                controller.animator.SetBool("isWalking", false);

                if (controller.seenPlayer && GameManager.instance.playerController.currentStealth < controller.detection)
                {
                    controller.SetBehavior(EnemyController.Behavior.Action); // Engage
                }
                else
                {
                    controller.SetBehavior(EnemyController.Behavior.Search); // Begin search
                }
            }
        }
    }