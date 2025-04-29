using UnityEngine;

public class StoreTop : MonoBehaviour
{
    [SerializeField] Transform[] slots;
    [SerializeField] GameObject[] items;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [ContextMenu("Storage/Spawn Children")]
    private void PopulateInEditor()
    {
        DeleteChidlren();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Length && items[i] != null)
            {
                Instantiate(items[i], slots[i].position, slots[i].rotation, slots[i]);
            }
        }
    }
    [ContextMenu("Storage/Delete Children")]
    private void DeleteChidlren()
    {
        foreach (Transform slot in slots)
        {
            for (int i = slot.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(slot.GetChild(i).gameObject);
            }
        }
    }
}
