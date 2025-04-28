using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] int capacity = 5;
    [SerializeField] Transform[] pocketSockets;
    [SerializeField] Transform handSocket;

    GameObject[] slots;
    ItemData[] slotData;
    int equipIndex = 0;
    int prevIndex = -1;
    private void Start()
    {
        slots = new GameObject[capacity];
        slotData = new ItemData[capacity];
    }
    public bool TryAdd(ItemData data)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = Instantiate(data.prefab, pocketSockets[i]);
                slots[i].transform.localPosition = Vector3.zero;
                slots[i].transform.localRotation = Quaternion.identity;
                if (slots[i].TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
                if (slots[i].TryGetComponent(out Collider col)) col.enabled = false;
                slotData[i] = data;
                return true;
            }
        }
        int currentEquip = equipIndex;
        int dropIndex;
        do { dropIndex = Random.Range(0, capacity); }
        while (dropIndex == currentEquip);
        Drop(dropIndex);
        return TryAdd(data);
    }
    void Drop(int index)
    {
        if (slots[index] == null || slotData[index] == null) return;
        GameObject pickup = Instantiate(slotData[index].emptyPickupPrefab);
        pickup.GetComponent<ItemPickup>().data = slotData[index];
        pickup.transform.position = slots[index].transform.position;
        pickup.transform.rotation = slots[index].transform.rotation;
        Rigidbody rb = pickup.GetComponent<Rigidbody>() ?? pickup.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(transform.forward * 2f + Vector3.up, ForceMode.Impulse);
        Destroy(slots[index]);
        slots[index] = null;
        slotData[index] = null;
    }
    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int dir = scroll > 0 ? 1 : -1;
            int newIndex = (equipIndex + dir + capacity) % capacity;
            Equip(newIndex);
        }
    }
    void Equip(int newIndex)
    {
        if (newIndex == equipIndex) return;
        if (slots[equipIndex] != null)
        {
            slots[equipIndex].transform.SetParent(pocketSockets[equipIndex]);
            slots[equipIndex].transform.localPosition = Vector3.zero;
            slots[equipIndex].transform.localRotation = Quaternion.identity;
        }
        if (slots[newIndex] != null)
        {
            slots[newIndex].transform.SetParent(handSocket);
            slots[newIndex].transform.localPosition = Vector3.zero;
            slots[newIndex].transform.localRotation = Quaternion.identity;
        }
        equipIndex = newIndex;
    }

    public bool Search(string name)
    {
        return false;
    }
}
