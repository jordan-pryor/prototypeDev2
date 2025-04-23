using UnityEngine;
[ExecuteAlways]
public class camController : MonoBehaviour
{
    [SerializeField] MonoBehaviour camSwivel;
    [SerializeField] MonoBehaviour camFPS;
    [SerializeField] PlayerController player;
    [SerializeField] GameObject neckBone;
    [SerializeField] float vertOff = 0.5f;
    private void Start()
    {
        ToggleCam();
    }
    private void OnValidate()
    {
        ToggleCam();
    }
    void LateUpdate()
    {
        Vector3 offset = new Vector3(0, vertOff, 0);
        transform.position = Vector3.Lerp(transform.position, neckBone.transform.position + offset, Time.deltaTime * 10 );
    }
    public void ToggleCam()
    {
        if (camSwivel != null && camFPS != null)
        {
            camSwivel.enabled = !player.isFPS;
            camFPS.enabled = player.isFPS;
        }
    }
}
