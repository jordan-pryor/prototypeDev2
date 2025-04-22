using UnityEngine;
[ExecuteAlways]
public class camController : MonoBehaviour
{
    [SerializeField] MonoBehaviour camSwivel;
    [SerializeField] MonoBehaviour camFPS;
    [SerializeField] PlayerController player;
    private void Start()
    {
        ToggleCam();
    }
    private void OnValidate()
    {
        ToggleCam();
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
