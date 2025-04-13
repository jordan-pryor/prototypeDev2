using UnityEngine;

public class cameraController : MonoBehaviour
{
    // === Settings (Serialized) ===
    [SerializeField] int sens = 100;            // Mouse sensitivity
    [SerializeField] int lockVertMin = -60;     // Min vertical angle
    [SerializeField] int lockVertMax = 60;      // Max vertical angle
    [SerializeField] bool invertY = false;      // Invert Y-axis?

    // === Internals ===
    float rotX = 0f;                             // Rotation around X axis

    // === Init ===
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;  // Lock and hide cursor
    }

    // === Camera Look Logic ===
    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

        // Adjust vertical rotation
        rotX += invertY ? mouseY : -mouseY;
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // Apply vertical look
        transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);

        // Apply horizontal look to parent (usually player body)
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
