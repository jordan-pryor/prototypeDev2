using UnityEngine;

public class cameraMovement : MonoBehaviour
{
	// === System References ===
	[Header("System References")]
	public Transform cameraPosition;                        // The target spot where our camera should chill – usually a child of the player

	// Update is called once per frame
	void Update()
	{
		transform.position = cameraPosition.position;       // Snap camera to follow the target – smooth brains, smooth camera
	}
}