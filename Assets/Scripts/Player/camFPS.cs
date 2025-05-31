using UnityEngine;

public class camFPS : MonoBehaviour
{
    [SerializeField] PlayerController player; // Reference to player for sensitivity and turning

    private float pitch = 0f;                 // Vertical camera angle (up/down)
    public float Pitch => pitch;
    public void SetPitch(float newPitch)
    {
        pitch = newPitch;
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
    void Update()
    {
        // Ignore input if cursor is not locked
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // Get mouse movement input
        float mouseX = Input.GetAxis("Mouse X") * player.sensX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * player.sensY * Time.deltaTime;

        // Adjust vertical pitch based on mouse Y
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -player.pitchClamp, player.pitchClamp);

        // Apply pitch to camera's local rotation
        transform.localRotation = Quaternion.Euler(pitch, 0, 0);

        // Rotate entire player body left/right on Y-axis using mouse X
        player.transform.Rotate(Vector3.up * mouseX);
    }
}
