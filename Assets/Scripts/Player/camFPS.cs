using UnityEngine;

public class camFPS : MonoBehaviour
{
    [SerializeField] private PlayerController player;  // Reference to player for input/sensitivity

    private float pitch = 0f;                           // Vertical look angle (up/down)

    void Update()
    {
        // Ignore input if mouse cursor isn't locked
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // Get scaled mouse input
        float mouseX = Input.GetAxis("Mouse X") * player.sensX * 0.01f;
        float mouseY = Input.GetAxis("Mouse Y") * player.sensY * 0.01f;

        // Update vertical angle (pitch)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -player.pitchClamp, player.pitchClamp);

        // Apply pitch to the camera (local)
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // Apply horizontal rotation (yaw) to the player root
        player.transform.Rotate(Vector3.up * mouseX);
    }

    // Sync pitch from another system (e.g. swivel/explore cam)
    public void SyncFrom(float srcPitch)
    {
        pitch = srcPitch;
    }

    // Retrieve current pitch (for syncing to another cam)
    public float GetPitch()
    {
        return pitch;
    }
}
