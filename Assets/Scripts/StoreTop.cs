using UnityEngine;

public class StoreTop : MonoBehaviour
{
    [SerializeField] Transform[] slots;
    [SerializeField] GameObject[] items;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if ( i < items.Length && items[i] != null)
            {
                Instantiate(items[i], slots[i].position, slots[i].rotation, slots[i]);
            }
        }
    }
}
