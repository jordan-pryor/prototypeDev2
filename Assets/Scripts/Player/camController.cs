using UnityEngine;
[ExecuteAlways]
public class camController : MonoBehaviour
{
    [SerializeField] MonoBehaviour camSwivel;
    [SerializeField] MonoBehaviour camFPS;
    [SerializeField] playerController player;

    // Update is called once per frame
    void Update()
    {
        if ( camSwivel != null && camFPS != null)
        {
            camSwivel.enabled = !player.isFPS;
            camFPS.enabled = player.isFPS;
        }
    }
    private void OnValidate()
    {
        if ( camSwivel != null && camFPS != null)
        {
            camSwivel.enabled = !player.isFPS;
            camFPS.enabled = player.isFPS;
        }
    }
}
