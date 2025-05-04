using UnityEngine;

public class RandomizeMaterial : MonoBehaviour
{
    [SerializeField] private Material[] mats; // Pool of materials to choose from
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();

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
}
