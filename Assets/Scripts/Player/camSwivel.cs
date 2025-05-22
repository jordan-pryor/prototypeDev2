using UnityEngine;

public class camSwivel : MonoBehaviour
{
    [SerializeField] PlayerController player; // Reference to player script

    private float pitch = 0f;                 // Vertical look angle
    private float yaw = 0f;                   // Horizontal look offset
    private Transform body;                   // Player's body transform

    private void Start()
    {
        body = player.transform;              // Cache player body reference
    }

    void Update()
    {
        // Get mouse input and apply sensitivity
        float mouseX = Input.GetAxis("Mouse X") * player.sensX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * player.sensY * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;

        // Clamp look angles
        pitch = Mathf.Clamp(pitch, -player.pitchClamp, player.pitchClamp);
        yaw = Mathf.Clamp(yaw, -player.yawClamp, player.yawClamp);

        // Apply pitch/yaw to camera rotation
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);

        // Determine how far player can look before body starts turning
        float turnThresh = player.yawClamp * (player.isSprinting ? player.turnThreshold * 0.5f : player.turnThreshold);
        float statThresh = player.yawClamp * player.stationaryThreshold;
        float curThresh = player.isMoving ? turnThresh : statThresh;

        // If yaw exceeds threshold, start rotating the body
        if (Mathf.Abs(yaw) >= curThresh)
        {
            float excess = yaw - Mathf.Sign(yaw) * curThresh;
            float speedMod = player.isSprinting ? player.sprintTurnMod : 0f;
            float maxTurn = (player.turnSpeed + speedMod) * Time.deltaTime;
            float turnStep = Mathf.Clamp(excess, -maxTurn, maxTurn);

            // Rotate player body and reduce yaw accordingly
            body.Rotate(Vector3.up * turnStep);
            yaw -= turnStep;

            // Reapply camera rotation with adjusted yaw
            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}