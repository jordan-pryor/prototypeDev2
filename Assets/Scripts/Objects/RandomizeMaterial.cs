using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using System.Collections.Generic;

public class RandomizeMaterial : MonoBehaviour
{
    [SerializeField] bool onStart = false;
    [SerializeField] bool onVal = false;
    [SerializeField] private Material[] mats; // Pool of materials to choose from
    [SerializeField] private List<int> ignoreSlots = new(); // Indexes to ignore

    [SerializeField] Renderer rend;
    private Material[] oldMats;

    private void Start()
    {
        CheckRend();
        Save();
        if (onStart)
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
        CheckRend();
        Randomize();
    }

    [ContextMenu("Save Materials")]
    private void EditorSave()
    {
        CheckRend();
        Save();
    }

    [ContextMenu("Reset Materials")]
    private void EditorReset()
    {
        CheckRend();
        Reset();
    }

    private void Randomize()
    {
        Reset();
        Save();
        if (mats == null || mats.Length == 0) return;

        Material[] newMats = new Material[rend.sharedMaterials.Length];

        for (int i = 0; i < newMats.Length; i++)
        {
            if (ignoreSlots.Contains(i))
            {
                newMats[i] = rend.sharedMaterials[i];
            }
            else
            {
                newMats[i] = mats[Random.Range(0, mats.Length)];
            }
        }

        rend.materials = newMats;
    }

    private void Reset()
    {
        if (oldMats != null)
        {
            rend.materials = oldMats;
        }
    }

    private void Save()
    {
        oldMats = rend.sharedMaterials;
    }

    private bool CheckRend()
    {
        if (rend != null) return true;
        else
        {
            rend = GetComponent<Renderer>();
            return true;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CheckRend();
        if (!Application.isPlaying && onVal)
        {
            Save();
            Reset();
            Randomize();
            //UnityEditor.EditorUtility.SetDirty(this); // Marks the object as dirty so the change persists
        }
    }
#endif
}