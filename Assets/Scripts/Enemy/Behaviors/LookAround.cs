using UnityEngine;

[CreateAssetMenu(fileName = "LookAround", menuName = "Enemy/Behaviors/LookAround")]
public class LookAround : EnemyBehavior, IEnemy
{
    private float timer;

    // Executes the search/look-around behavior
    public override void Execute(EnemyController controller)
    {
        controller.animator.SetTrigger("isSearching");  // Play search animation
        timer += Time.deltaTime;                        // Track search duration

        if (timer < controller.searchTime)
        {
            // Rotate eyes left and right in a sine wave pattern
            float angle = Mathf.Sin(Time.time * 2f) * 45f;
            controller.eyes[0].localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            // Reset rotation and signal search is done
            controller.eyes[0].localRotation = Quaternion.identity;
            controller.OnSearchEnd();
            timer = 0f;
        }
    }
}
