using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] int capacity = 5;
    [SerializeField] Transform[] pocketSockets;
    [SerializeField] Transform handSocket;

    public GameObject[] slots;
    BaseData[] slotData;
    const int None = -1;
    public int equipIndex = 0;
    private void Start()
    {
        slots = new GameObject[capacity];
        slotData = new BaseData[capacity];
    }
    public bool TryAdd(BaseData data)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (slots[i] != null) continue;
            //Ptll prefab
            slots[i] = Instantiate(data.prefab, pocketSockets[i]);
            slots[i].transform.localPosition = Vector3.zero;
            slots[i].transform.localRotation = data.defaultRotation;
            //init weapons
            if (data is WeaponData weaponData && slots[i].TryGetComponent(out Gun gun))
            {
                gun.PullStat(weaponData);
            }
            //disable physics
            if (slots[i].TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
            if (slots[i].TryGetComponent(out Collider col)) col.enabled = false;
            // store data
            slotData[i] = data;
            Equip(i);
            return true;
        }
        int dropIndex = 0;
        do { dropIndex = Random.Range(0, capacity); }
        while (dropIndex == equipIndex);
        Drop(dropIndex);
        return TryAdd(data);
    }
    public void Delete(int index)
    {
        if (slots[index] == null) return;
        //destroy and reset
        Destroy(slots[index]);
        slots[index] = null;
        slotData[index] = null;
        if (index == equipIndex)
        {
            equipIndex = None;
            GameManager.instance.playerController.SwitchCam(false);
        }
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
        Delete(index);
    }
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
                    Equip(dex); break;
                }
            }
        }
    }
    void Equip(int newIndex)
    {
        if (newIndex == equipIndex) return;
        if (equipIndex != None && slots[equipIndex] != null)
        {
            slots[equipIndex].transform.SetParent(pocketSockets[equipIndex]);
            slots[equipIndex].transform.localPosition = Vector3.zero;
            slots[equipIndex].transform.localRotation = slotData[equipIndex].defaultRotation;
        }
        if (slots[newIndex] != null)
        {
            slots[newIndex].transform.SetParent(handSocket);
            slots[newIndex].transform.localPosition = Vector3.zero;
            slots[newIndex].transform.localRotation = slotData[newIndex].defaultRotation;
        }
        equipIndex = newIndex;
        GameManager.instance.playerController.SwitchCam(CheckCam(equipIndex));
    }
    public bool Search(string name)
    {
        if (equipIndex == None) return false;
        if (slotData[equipIndex] != null && slotData[equipIndex].itemName == name)
        {
            Delete(equipIndex); return true;
        }
        else return false;
    }
    public bool CheckCam(int index)
    {
        return slotData[equipIndex] is WeaponData;
    }
}

/* USE ITEM
void useItem(BaseData data) {
    if (data is HealData)
    {
        // cast item as heal
        HealData heal = (HealData)data;
        useHealItem(heal.instantAmt, heal.hotAmt, heal.sec);
    }
    else if (data is TrapData)
    {
        // cast item as trap
        TrapData trap = (TrapData)data;
        if(controller.isGrounded) placeTrap(trap);
    }
}
*/

/*
 * PLACE TRAP
 * === if we can crouch, then instantiate, then uncrouch easily, that would add to the experience. if not, all good
*
void placeTrap(TrapData trap)
{
    // place trap at player pos
    Vector3 trapPos = transform.position;
    // change y to 0.075 according to model size
    trapPos.y = 0.075f;
    // crouch

    // instantiate trap.trap??
    Instantiate(trap.trapToSet, trapPos, Quaternion.identity);
    // uncrouch

    // empty hands
    itemModel.GetComponent<MeshFilter>().sharedMesh = null;
    itemModel.GetComponent<MeshRenderer>().sharedMaterial = null;

    // remove from inv
    inv.Remove(inv[invPos]);
    invPos = inv.Count - 1;
}
*/

/* USE HEAL ITEM
void useHealItem(int instantAmt, int hotAmt = 0, int sec = 1)
{
    // instant heal - doesn't count for HOT
    if (HP + instantAmt < origHP)
        HP += instantAmt;
    else
        HP = origHP; // full

    // hot
    if (hotAmt > 0 && HP < origHP) // not full health
    {
        // reset all hot vars
        // any previous hot is lost
        healCount = sec;
        healAmt = hotAmt;
    }

    // empty hands
    itemModel.GetComponent<MeshFilter>().sharedMesh = null;
    itemModel.GetComponent<MeshRenderer>().sharedMaterial = null;
    // remove from inv
    inv.Remove(inv[invPos]);
    invPos = inv.Count - 1;
}
*/
