using UnityEngine;

public class RandomizeMaterial : MonoBehaviour
{
    [SerializeField] private Material[] mats;
    private Renderer rend;
    private void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend == null || mats == null || mats.Length == 0)return;
        Material[] newMats = new Material[rend.sharedMaterials.Length];
        for (int i = 0; i < newMats.Length; i++)
        {
            newMats[i] = mats[Random.Range(0, mats.Length)];
        }
        rend.materials = newMats;
    }
}
