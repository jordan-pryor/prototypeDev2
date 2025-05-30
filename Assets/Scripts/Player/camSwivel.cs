using UnityEngine;

public class camSwivel : MonoBehaviour
{
    [SerializeField] private PlayerController player;  // Player reference

    private float pitch = 0f;                           // Look up/down
    private float yaw = 0f;                             // Look left/right
    private Transform body;                             // Reference to player body

    private void Start()
    {
        body = player.transform;                        // Cache body reference
    }

    void Update()
    {
        // Skip if cursor isn't locked (pause, UI, etc.)
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // Read input and scale by sensitivity
        float mouseX = Input.GetAxis("Mouse X") * player.sensX * 0.01f;
        float mouseY = Input.GetAxis("Mouse Y") * player.sensY * 0.01f;

        // Update angles
        yaw += mouseX;
        pitch -= mouseY;

        // Clamp look angles
        pitch = Mathf.Clamp(pitch, -player.pitchClamp, player.pitchClamp);
        yaw = Mathf.Clamp(yaw, -player.yawClamp, player.yawClamp);

        // Apply local rotation to camera
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);

        // Decide yaw threshold based on movement
        float curThreshold = player.isMoving
            ? player.yawClamp * player.turnThreshold
            : player.yawClamp * player.stationaryThreshold;

        // Start turning body if yaw exceeds threshold
        if (Mathf.Abs(yaw) > curThreshold)
        {
            float excess = yaw - Mathf.Sign(yaw) * curThreshold;
            float speedMod = player.isSprinting ? player.sprintTurnMod : 0f;
            float maxTurn = (player.turnSpeed + speedMod) * Time.deltaTime;

            float turnStep = Mathf.Clamp(excess, -maxTurn, maxTurn);

            // Rotate player body and reduce yaw
            body.Rotate(Vector3.up * turnStep);
            yaw -= turnStep;

            // Re-apply corrected cam angle after body turn
            transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }

    // Sync pitch when switching from FPS camera
    public void SyncFrom(float srcPitch)
    {
        pitch = srcPitch;
    }

    // Expose pitch to FPS camera
    public float GetPitch()
    {
        return pitch;
    }
}
