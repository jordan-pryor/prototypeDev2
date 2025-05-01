using UnityEngine;
public class SenseTrigger : MonoBehaviour
{
    public enum SenseType { Sight, Smell, Hearing, Action};
    [Header("References")]
    public SenseType sense;
    private EnemyController enemy;
    void Awake()
    {
        enemy = GetComponentInParent<EnemyController>();
    }
    private void Update()
    {
        if(sense == SenseType.Sight || sense == SenseType.Action)
        {
            CastSense();
        }
    }
    private void CastSense()
    {
        Transform[] eyes = sense == SenseType.Sight ? enemy.eyes : enemy.actionOrigins;
        float range = sense == SenseType.Sight ? enemy.sightRange : enemy.actionRange;
        float fov = sense == SenseType.Sight ? enemy.sightFOV : enemy.actionFOV;
        LayerMask mask = sense == SenseType.Sight ? enemy.sightMask : enemy.actionMask;
        bool detect = false;
        //Debug.DrawLine(eye.position, eye.position + (direction * range), Color.yellow);
        foreach (Transform eye in eyes)
        {
            Transform player = GameManager.instance.player.transform;
            Vector3 playerDir = player.position - eye.position;
            float playerDis = playerDir.magnitude;
            Debug.DrawLine(eye.position, eye.position + eye.forward * range, Color.green);
            Vector3 left = Quaternion.Euler(0, -fov * 0.5f, 0) * eye.forward;
            Vector3 right = Quaternion.Euler(0, fov * 0.5f, 0) * eye.forward;
            Debug.DrawLine(eye.position, eye.position + left * range, Color.yellow);
            Debug.DrawLine(eye.position, eye.position + right * range, Color.yellow);
            Debug.DrawLine(eye.position, player.position, Color.blue);
            if (playerDis < range)
            {
                float angle = Vector3.Angle(eye.forward, playerDir.normalized);
                if(angle < fov)
                {

                    if (Physics.Raycast(eye.position, playerDir.normalized, out RaycastHit hit, range, mask, QueryTriggerInteraction.Ignore))
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            if (GameManager.instance.playerController.currentStealth >= enemy.detection) return;
                            detect = true;
                            if (sense == SenseType.Sight)
                            {
                                enemy.seenPlayer = true;
                                enemy.RayTrigger(hit.collider);
                            }
                        }
                    }
                }
            }
        }
        if (sense == SenseType.Sight && !detect) enemy.seenPlayer = false;
    }

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