using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] GameObject lightRef;
    [SerializeField] Renderer rendererRef;
    [SerializeField] Material baseMat;
    [SerializeField] Material emissMat;
    [SerializeField] bool isOn = false;

    // Update is called once per frame
    void Update()
    {
        CheckLight();
    }
    private void OnValidate()
    {
        CheckLight();
    }

    void CheckLight()
    {
        Material[] mats = rendererRef.materials;
        if (isOn)
        {
            lightRef.SetActive(true);
            mats[1] = emissMat;
        }
        else
        {
            lightRef.SetActive(false);
            mats[1] = baseMat;
        }
        rendererRef.materials = mats;
    }
}
