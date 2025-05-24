using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Flashlight : MonoBehaviour, IInteract, IUse
{
    [SerializeField] GameObject lightRef;           // Actual light object
    [SerializeField] Material baseMat;              // Non-emissive material
    [SerializeField] Material emissiveMat;          // Emissive/glowing material
    [SerializeField] bool isOn = false;             // Current on/off state
    [SerializeField] int matIndex = 0;              // Index of material slot to change
    [SerializeField] bool isInteractable = true;
    [SerializeField] bool isFlicker = false;
    [SerializeField] float flickerTime = 0f;
    [SerializeField] Renderer rendererRef;          // Renderer used to swap materials
    Material[] materials;
    private Coroutine flickerRoutine;

    private void Start()
    {
        // Get renderer and set initial material/light state
        rendererRef = GetComponent<Renderer>();
        if (rendererRef != null)
            materials = rendererRef.sharedMaterials;

        ApplyLight();
        if (isFlicker)
            flickerRoutine = StartCoroutine(Flicker());
    }

    private void OnValidate()
    {
        // Auto-update visuals when values are changed in editor
        ApplyLight();
    }

    // Applies light and material state based on `isOn`
    void ApplyLight()
    {
        if (lightRef != null) lightRef.SetActive(isOn);

        if (rendererRef != null && rendererRef.sharedMaterials.Length > matIndex)
        {
            Material[] mats = rendererRef.sharedMaterials;
            mats[matIndex] = isOn ? emissiveMat : baseMat;
            rendererRef.sharedMaterials = mats;
        }
    }

    // Public method to toggle flashlight state
    public void ToggleLight()
    {
        isOn = !isOn;
        ApplyLight();
    }

    // Called when interacted with (e.g. pickup or activation)
    public void Interact()
    {
        if(isInteractable) ToggleLight();
    }

    // Called via item usage system (e.g. equipped item usage)
    public void Use(bool primary)
    {
        if (primary)
        {
            ToggleLight();
        }
    }

    private IEnumerator Flicker()
    {
        while (isFlicker)
        {
            ToggleLight();
            yield return new WaitForSeconds(Random.Range(flickerTime - 0.1f, flickerTime + 0.1f));
        }
    }
}
