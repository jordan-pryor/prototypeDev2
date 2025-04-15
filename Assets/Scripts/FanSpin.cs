using UnityEngine;

public class FanSpin : MonoBehaviour
{
    [SerializeField] bool isOn;
    [SerializeField] GameObject blades;
    [SerializeField] float fanSpeed = 10f;

    // Update is called once per frame
    void Update()
    {
        if(isOn)
        {
            blades.transform.Rotate(Vector3.forward, fanSpeed * Time.deltaTime, Space.Self);
        }
    }
}
