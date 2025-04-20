using UnityEngine;

public class camFPS : MonoBehaviour
{
    [SerializeField] playerController player;
    private float pitch = 0f;

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

        // Handle horizontal head turning
        pitch -= mouseY;
        // clamp cam on x-axis
        pitch = Mathf.Clamp(pitch, -player.pitchClamp, player.pitchClamp);

        // Apply rotations
        transform.localRotation = Quaternion.Euler(pitch, 0, 0);

        // rotate the player on the y-axis to look left and right
        player.transform.Rotate(Vector3.up * mouseX);
    }
}
