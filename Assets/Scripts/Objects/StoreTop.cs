using UnityEngine;

public class StoreTop : MonoBehaviour
{
    [SerializeField] Transform[] slots;     // Locations to spawn items
    [SerializeField] GameObject[] items;    // Item prefabs to place into slots

    // Called manually in editor to populate slots with items
    [ContextMenu("Storage/Spawn Children")]
    private void PopulateInEditor()
    {
        DeleteChidlren(); // Clean up existing children first

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Length && items[i] != null)
            {
                // Instantiate item at slot position, parented to the slot
                Instantiate(items[i], slots[i].position, slots[i].rotation, slots[i]);
            }
        }
    }

    // Called manually in editor to clear existing spawned children
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
