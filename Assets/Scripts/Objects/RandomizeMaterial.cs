using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class RandomizeMaterial : MonoBehaviour
{
    [SerializeField] bool isEnabled = false;
    [SerializeField] private Material[] mats; // Pool of materials to choose from
    private Renderer rend;
    private Material[] oldMats;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        Save();
        if ( isEnabled)
        {
            Randomize();
        }
        else
        {
            Reset();
        }
    }
    [ContextMenu("Randomize Materials")]
    private void EditorRandomize()
    {
        rend = GetComponent<Renderer>();
        Randomize();
    }

    [ContextMenu("Save Materials")]
    private void EditorSave()
    {
        rend = GetComponent<Renderer>();
        Save();
    }
    [ContextMenu("Reset Materials")]
    private void EditorReset()
    {
        rend = GetComponent<Renderer>();
        Reset();
    }
    private void Randomize()
    {
        Reset();
        Save();
        // If no renderer or no materials to choose from, exit
        if (rend == null || mats == null || mats.Length == 0) return;

        // Create a new array matching the size of current material slots
        Material[] newMats = new Material[rend.sharedMaterials.Length];

        // Randomly assign a material to each slot
        for (int i = 0; i < newMats.Length; i++)
        {
            newMats[i] = mats[Random.Range(0, mats.Length)];
        }

        rend.materials = newMats; // Apply the randomized materials
    }
    private void Reset()
    {
        if (rend != null && oldMats != null)
        {
            rend.materials = oldMats;
        }
    }
    private void Save()
    {
        oldMats = rend.sharedMaterials;
    }
}
