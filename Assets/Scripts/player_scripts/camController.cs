using UnityEngine;
[ExecuteAlways]
public class camController : MonoBehaviour
{
    [SerializeField] MonoBehaviour camSwivel;
    [SerializeField] MonoBehaviour camFPS;
    [SerializeField] playerController player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        camSwivel.enabled = !player.isFPS;
        camFPS.enabled = player.isFPS;
    }
    private void OnValidate()
    {
        camSwivel.enabled = !player.isFPS;
        camFPS.enabled = player.isFPS;
    }
}
