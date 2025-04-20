using UnityEngine;
public class SenseTrigger : MonoBehaviour
{
    public enum SenseType { Sight, Smell, Hearing, Action};
    public SenseType sense;
    private EnemyController enemy;

    void Awake()
    {
        enemy = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemy == null) return;

        switch (sense)
        {
            case SenseType.Sight:
                enemy.OnSightTrigger(this, other);
                break;

            case SenseType.Hearing:
                enemy.OnHearingTrigger(this, other);
                break;

            case SenseType.Smell:
                enemy.OnSmellTrigger(this, other);
                break;

            case SenseType.Action:
                enemy.OnActionTrigger(this, other);
                break;
        }
    }
}