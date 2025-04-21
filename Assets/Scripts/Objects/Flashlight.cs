using UnityEngine;
using UnityEngine.UIElements;

public class Flashlight : MonoBehaviour, IInteract
{
    [SerializeField] GameObject lightRef;
    [SerializeField] Material baseMat;
    [SerializeField] Material emissiveMat;
    [SerializeField] bool isOn = false;
    [SerializeField] int matIndex = 0;

    Renderer rendererRef;
    Material[] materials;
    private void Start()
    {
        // Start state
        rendererRef = GetComponent<Renderer>();
        if (rendererRef != null) materials = rendererRef.materials;
        ApplyLight();
    }
    private void OnValidate()
    {
        // Update in Editor
        ApplyLight();
    }
    void ApplyLight()
    {
        // Toggle Light and switch Materials
        if (lightRef != null) lightRef.SetActive(isOn);
        if (materials != null && materials.Length > matIndex)
        {
            materials[matIndex] = isOn ? emissiveMat : baseMat;
            rendererRef.materials = materials;
        }
    }
    public void ToggleLight()
    {
        // Toggles light instead of Update
        isOn = !isOn;
        ApplyLight();
    }
    public void Interact()
    {
        // Calls Toggle
        ToggleLight();
    }

    // Script for using items
    //{
        // Calls Toggle
     //   ToggleLight();
    //}
}
