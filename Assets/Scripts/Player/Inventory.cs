using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] int capacity = 5;                   // Max number of items
    [SerializeField] Transform[] pocketSockets;          // Storage positions for inventory items
    [SerializeField] Transform handSocket;               // Socket for equipped item
    [SerializeField] private float placementDistance = 2f;

    public GameObject[] slots;                           // Instantiated item objects
    BaseData[] slotData;                                 // ScriptableObject data for each slot

    const int None = -1;                                 // Constant representing no equipped item
    public int equipIndex = None;                        // Currently equipped slot index
    struct MaterialData
    {
        public string name;
        public int amount;
        public MaterialData(string name, int amount)
        {
            this.name = name;
            this.amount = amount;
        }
    }
    [SerializeField] private List<MaterialData> materials;

    private void Start()
    {
        slots = new GameObject[capacity];
        slotData = new BaseData[capacity];
        equipIndex = None;
        materials = new List<MaterialData>();
    }

    // Tries to add a new item to the inventory
    public bool TryAdd(BaseData data)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (slots[i] != null) continue;

            // Spawn the item in the pocket slot
            slots[i] = Instantiate(data.prefab, pocketSockets[i]);
            slots[i].transform.localPosition = Vector3.zero;
            slots[i].transform.localRotation = data.defaultRotation;

            // Initialize weapon if applicable
            if (data is WeaponData weaponData && slots[i].TryGetComponent(out Gun gun))
            {
                gun.PullStat(weaponData);
            }
            else if (data is TrapData trapData && slots[i].TryGetComponent(out Trap trap))
            {
                trap.PullStat(trapData);
            }

            // Disable physics
            if (slots[i].TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
            if (slots[i].TryGetComponent(out Collider col)) col.enabled = false;

            slotData[i] = data;
            Equip(i);  // Auto-equip
            return true;
        }

        // Inventory full — drop a random non-equipped item and try again
        int dropIndex;
        do { dropIndex = Random.Range(0, capacity); }
        while (dropIndex == equipIndex);

        Drop(dropIndex);
        return TryAdd(data);
    }

    // Deletes the item at a given index
    public void Delete(int index)
    {
        if (slots[index] == null) return;
        Destroy(slots[index]);
        slots[index] = null;
        slotData[index] = null;

        if (index == equipIndex)
        {
            equipIndex = None;
            GameManager.instance.playerController.SwitchCam(false);
        }
    }

    // Drops the item at the given index into the world
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

        Delete(index);
    }

    // Handles input for scrolling to switch equipped item
    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int dir = scroll > 0 ? 1 : -1;
            int start = equipIndex == None ? 0 : equipIndex;

            for (int i = 1; i <= capacity; i++)
            {
                int dex = (start + dir * i + capacity) % capacity;
                if (slots[dex] != null)
                {
                    Equip(dex);
                    break;
                }
            }
        }
    }

    // Equips the item at the given index
    void Equip(int newIndex)
    {
        if (newIndex == equipIndex) return;

        // Move current equipped item back to pocket
        if (equipIndex != None && slots[equipIndex] != null)
        {
            slots[equipIndex].transform.SetParent(pocketSockets[equipIndex]);
            slots[equipIndex].transform.localPosition = Vector3.zero;
            slots[equipIndex].transform.localRotation = slotData[equipIndex].defaultRotation;
        }

        // Move new item to hand
        if (slots[newIndex] != null)
        {
            slots[newIndex].transform.SetParent(handSocket);
            slots[newIndex].transform.localPosition = Vector3.zero;
            slots[newIndex].transform.localRotation = slotData[newIndex].defaultRotation;
        }

        equipIndex = newIndex;
        GameManager.instance.playerController.SwitchCam(CheckCam(equipIndex));
    }

    // Unequips the current item
    public void Unequip()
    {
        if (equipIndex == None || slots[equipIndex] == null) return;

        slots[equipIndex].transform.SetParent(pocketSockets[equipIndex]);
        slots[equipIndex].transform.localPosition = Vector3.zero;
        slots[equipIndex].transform.localRotation = slotData[equipIndex].defaultRotation;

        equipIndex = None;
        GameManager.instance.playerController.SwitchCam(false);
    }

    // Deletes item if equipped item's name matches
    public bool Search(string name)
    {
        if (equipIndex == None) return false;

        if (slotData[equipIndex] != null && slotData[equipIndex].itemName == name)
        {
            Delete(equipIndex);
            return true;
        }

        return false;
    }

    // Checks if the item should use first-person camera
    public bool CheckCam(int index)
    {
        return slotData[equipIndex] is WeaponData;
    }    
    /*
    void PlaceTrap(TrapData trap)
    {
        Vector3 trapPos = GameManager.instance.player.transform.position;
        trapPos.y = 0.075f;
        GameObject trapObj = Instantiate(trap.trapToSet, trapPos, Quaternion.identity);
        if(trapObj.TryGetComponent<Trap>(out var trapcomp))
        {
            trapcomp.PullStat(trap);
        }
        Delete(equipIndex);
    }
    */
    public void PlaceTrap(int index)
    {
        if (slots[index] == null || slotData[index] == null) return;

        // Instantiate trap prefab
        GameObject trap = Instantiate(slotData[index].emptyPickupPrefab);
        trap.GetComponent<ItemPickup>().data = slotData[index];
        if (slotData[index] is TrapData trapData)
        {
            if (trap.TryGetComponent<Trap>(out var trapComp))
            {
                trapComp.PullStat(trapData);
            }
        }
        // Calculate placement position (in front of player, on the ground)
        Vector3 forwardPosition = Camera.main.transform.position + Camera.main.transform.forward * placementDistance;
        RaycastHit hit;

        // Raycast downward to align with the ground
        if (Physics.Raycast(forwardPosition + Vector3.up, Vector3.down, out hit, 5f))
        {
            trap.transform.position = hit.point;
        }
        else
        {
            // Fallback to forward position if ground not detected
            trap.transform.position = forwardPosition;
        }

        // No rotation (aligned to world space)
        trap.transform.rotation = Quaternion.identity;

        // Remove rigidbody or make it static since it's a placed trap
        Rigidbody rb = trap.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        // Remove from inventory
        Delete(index);
    }
}


