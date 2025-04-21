using UnityEngine;

public class camSwivel : MonoBehaviour
{
    [SerializeField] playerController player;
    private float pitch = 0f;
    private float yaw = 0f;
    private int speedMod = 45;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   // Trap the cursor – no escape!
        Cursor.visible = false;                     // Hide the cursor – it's immersion time
    }
    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * player.sensX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * player.sensY * Time.deltaTime;

        // Handle vertical head turning
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -player.pitchClamp, player.pitchClamp);

        // Handle horizontal head turning
        yaw += mouseX;
        yaw = Mathf.Clamp(yaw, -player.yawClamp, player.yawClamp);

        // Apply rotations
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);

        // turning the body
        float turnThresh;
        if ( player.isSprinting)
        {
            turnThresh = player.yawClamp * ( player.turnThreshold / 2 );
            speedMod = 45;
        }
        else
        {
            turnThresh = player.yawClamp * player.turnThreshold;
            speedMod = 0;
        }
        float stationaryThresh = player.yawClamp * player.stationaryThreshold;
        if (player.isMoving)
        {
            if (Mathf.Abs(yaw) >= turnThresh)
            {
                float excess = yaw - Mathf.Sign(yaw) * turnThresh;
                // limit turn using turn speed
                float maxTurn = ( player.turnSpeed + speedMod) * Time.deltaTime;
                float turnStep = Mathf.Clamp(excess, -maxTurn, maxTurn);
                player.transform.Rotate(Vector3.up * turnStep);
                yaw -= turnStep;
                transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
            }
        }
        else
        {
            if (Mathf.Abs(yaw) >= stationaryThresh)
            {
                float excess = yaw - Mathf.Sign(yaw) * stationaryThresh;
                // limit turn using turn speed
                float maxTurn = (player.turnSpeed + speedMod) * Time.deltaTime;
                float turnStep = Mathf.Clamp(excess, -maxTurn, maxTurn);
                player.transform.Rotate(Vector3.up * turnStep);
                yaw -= turnStep;
                transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
            }
        }
        // If at max yaw then rotate the body
        
    }
}