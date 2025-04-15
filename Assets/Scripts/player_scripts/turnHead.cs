using UnityEngine;

public class turnHead : MonoBehaviour
{
    [SerializeField] playerController player;
    private float pitch = 0f;
    private float yaw = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;   // Trap the cursor – no escape!
        Cursor.visible = false;                     // Hide the cursor – it's immersion time
    }
    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * player.sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * player.sens * Time.deltaTime;

        // Handle horizontal head turning
        yaw -= mouseY;
        // clamp cam on x-axis
        yaw = Mathf.Clamp(yaw, -player.pitchClamp, player.pitchClamp);

        // Apply rotations
        transform.localRotation = Quaternion.Euler(yaw, 0, 0);

        // rotate the player on the y-axis to look left and right
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
