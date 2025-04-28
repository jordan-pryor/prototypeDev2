using UnityEngine;

public class camSwivel : MonoBehaviour
{
    [SerializeField] PlayerController player;
    private float pitch = 0f;
    private float yaw = 0f;
    private Transform body;
    private void Start()
    {
        body = player.transform;
    }
    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * player.sensX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * player.sensY * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -player.pitchClamp, player.pitchClamp);
        yaw = Mathf.Clamp(yaw, -player.yawClamp, player.yawClamp);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);

        float turnThresh = player.yawClamp * (player.isSprinting ? player.turnThreshold * 0.5f : player.turnThreshold);
        float statThresh = player.yawClamp * player.stationaryThreshold;
        float curThresh = player.isMoving ? turnThresh : statThresh;

        if (Mathf.Abs(yaw) >= curThresh)
        {
            float excess = yaw - Mathf.Sign(yaw) * curThresh;
            float speedMod = player.isSprinting ? player.sprintTurnMod : 0f;
            float maxTurn = (player.turnSpeed + speedMod) * Time.deltaTime;
            float turnStep = Mathf.Clamp(excess, -maxTurn, maxTurn);

            body.Rotate(Vector3.up * turnStep);
            yaw -= turnStep;
            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}