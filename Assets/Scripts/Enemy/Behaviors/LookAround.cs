using UnityEngine;

[CreateAssetMenu(fileName = "LookAround", menuName = "Enemy/Behaviors/LookAround")]
public class LookAround : EnemyBehavior, IEnemy
{
    private float timer;
    public override void Execute(EnemyController controller)
    {
        controller.animator.SetTrigger("isSearching");
        timer += Time.deltaTime;
        if(timer < controller.searchTime)
        {
            float angle = Mathf.Sin(Time.time * 2f) * 45f;
            controller.eyes[0].localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        else
        {
            controller.eyes[0].localRotation = Quaternion.identity;
            controller.OnSearchEnd();
            timer = 0f;
        }
    }
}
