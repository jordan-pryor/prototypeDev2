using UnityEngine;
public class SenseTrigger : MonoBehaviour
{
    public enum SenseType { Sight, Smell, Hearing, Action };

    [Header("References")]
    public SenseType sense;                    // Type of sense this trigger uses
    private EnemyController enemy;            // Reference to parent enemy controller

    void Awake()
    {
        enemy = GetComponentInParent<EnemyController>();  // Get enemy controller in parent object
    }

    private void Update()
    {
        // Sight and action senses use raycasts every frame
        if (sense == SenseType.Sight || sense == SenseType.Action)
        {
            CastSense();
        }
    }

    // Casts rays from eyes or action origins to detect the player
    private void CastSense()
    {
        Transform[] eyes = sense == SenseType.Sight ? enemy.eyes : enemy.actionOrigins;
        float range = sense == SenseType.Sight ? enemy.sightRange : enemy.actionRange;
        float fov = sense == SenseType.Sight ? enemy.sightFOV : enemy.actionFOV;
        LayerMask mask = sense == SenseType.Sight ? enemy.sightMask : enemy.actionMask;

        bool detect = false;

        foreach (Transform eye in eyes)
        {
            Transform player = GameManager.instance.player.transform;
            Vector3 playerDir = player.position - eye.position;
            float playerDis = playerDir.magnitude;

            // Debug visualization
            Debug.DrawLine(eye.position, eye.position + eye.forward * range, Color.green);
            Vector3 left = Quaternion.Euler(0, -fov * 0.5f, 0) * eye.forward;
            Vector3 right = Quaternion.Euler(0, fov * 0.5f, 0) * eye.forward;
            Debug.DrawLine(eye.position, eye.position + left * range, Color.yellow);
            Debug.DrawLine(eye.position, eye.position + right * range, Color.yellow);
            Debug.DrawLine(eye.position, player.position, Color.blue);

            if (playerDis < range)
            {
                float angle = Vector3.Angle(eye.forward, playerDir.normalized);
                if (angle < fov)
                {
                    // Check line of sight using raycast
                    if (Physics.Raycast(eye.position, playerDir.normalized, out RaycastHit hit, range, mask, QueryTriggerInteraction.Ignore))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            // Only detect if player stealth is below detection threshold
                            if (GameManager.instance.playerController.currentStealth >= enemy.detection) return;

                            detect = true;

                            if (sense == SenseType.Sight)
                            {
                                enemy.seenPlayer = true;
                                enemy.RayTrigger(hit.collider); // Trigger AI reaction
                            }
                        }
                    }
                }
            }
        }

        // Reset seenPlayer if no detection
        if (sense == SenseType.Sight && !detect) enemy.seenPlayer = false;
    }

    // Trigger-based sensory input (hearing/smell)
    private void OnTriggerEnter(Collider other)
    {
        if (enemy == null) return;

        switch (sense)
        {
            case SenseType.Hearing:
                enemy.OnHearingTrigger(this, other);
                break;

            case SenseType.Smell:
                enemy.OnSmellTrigger(this, other);
                break;
        }
    }
}