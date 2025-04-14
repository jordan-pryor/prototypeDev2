using UnityEngine;

public class cameraController : MonoBehaviour
{
	// === Camera Settings ===
	[Header("Camera Settings")]
	public float sensX;                             // Horizontal sensitivity – how fast we look side to side
	public float sensY;                             // Vertical sensitivity – how fast we look up and down

	// === System References ===
	[Header("System References")]
	public Transform orientation;                   // This rotates the player’s direction, separate from head tilt

	// === Internal Rotation State ===
	float xRotation;                                // Current vertical look angle (pitch)
	float yRotation;                                // Current horizontal look angle (yaw)

	// Start is called before the first frame update
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;   // Trap the cursor – no escape!
		Cursor.visible = false;                     // Hide the cursor – it's immersion time
	}

	// Update is called once per frame
	void Update()
	{
		float mouseX = Input.GetAxis("Mouse X") * sensX;  // Mouse movement left/right
		float mouseY = Input.GetAxis("Mouse Y") * sensY;  // Mouse movement up/down

		yRotation += mouseX;                              // Add horizontal movement to yaw
		xRotation -= mouseY;                              // Subtract vertical to get proper pitch

		xRotation = Mathf.Clamp(xRotation, -90f, 90f);     // No full neck spins – clamp the vertical look

		transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);      // Rotate the camera (head)
		orientation.rotation = Quaternion.Euler(0, yRotation, 0);            // Rotate the player (body)
	}
}