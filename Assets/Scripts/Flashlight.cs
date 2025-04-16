using UnityEngine;
using UnityEngine.UIElements;

public class Flashlight : MonoBehaviour
{
    [SerializeField] GameObject lightRef;
    [SerializeField] Material baseMat;
    [SerializeField] Material emissMat;
    [SerializeField] bool isOn = false;
    [SerializeField] int matIndex;

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
        Renderer rendererRef = GetComponent<Renderer>();
        Material[] mats = rendererRef.sharedMaterials;
        if (isOn)
        {
            lightRef.SetActive(true);
            mats[matIndex] = emissMat;
        }
        else
        {
            lightRef.SetActive(false);
            mats[matIndex] = baseMat;
        }
        rendererRef.sharedMaterials = mats;
    }

    public void Interact()
    {
        isOn = !isOn;
    }
}
