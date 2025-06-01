using UnityEngine;
[ExecuteAlways]
public class camController : MonoBehaviour
{
    [SerializeField] MonoBehaviour camSwivel;     // Third-person camera script
    [SerializeField] MonoBehaviour camFPS;        // First-person camera script
    [SerializeField] PlayerController player;     // Reference to player for cam mode
    [SerializeField] GameObject neckBone;         // Follow target (neck position)
    [SerializeField] float vertOff = 0.5f;        // Vertical offset above neck

    private void Start()
    {
        ToggleCam(); // Initialize camera mode on start
    }

    void LateUpdate()
    {
        // Smoothly follow the neck with a slight vertical offset
        Vector3 offset = new Vector3(0, vertOff, 0);
        transform.position = Vector3.Lerp(
            transform.position,
            neckBone.transform.position + offset,
            Time.deltaTime * 10
        );
    }

    // Enables appropriate camera script based on isFPS
    public void ToggleCam()
    {
        if (player.isFPS)
        {
            var swivel = camSwivel as camSwivel;
            float spitch = swivel.Pitch;
            float syaw = swivel.Yaw;
            player.transform.Rotate(Vector3.up * syaw);
            var fps = camFPS as camFPS;
            fps.SetPitch(spitch);
        }
        else
        {
            var fps = camFPS as camFPS;
            float fpitch = fps.Pitch;
            float bodyYaw = player.transform.eulerAngles.y;
            float camYaw = camFPS.transform.rotation.eulerAngles.y;
            float deltaYaw = Mathf.DeltaAngle(bodyYaw, camYaw);
            var swivel = camSwivel as camSwivel;
            swivel.SetPitchYaw(fpitch, deltaYaw);
        }
        camSwivel.enabled = !player.isFPS;
        camFPS.enabled = player.isFPS;
    }
}
