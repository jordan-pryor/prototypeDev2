using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteract
{
    public BaseData data;        // Item data (can be weapon or general object)
    private GameObject vis;      // Visual representation of the item

    private void Start()
    {
        // Remove any existing visuals and instantiate the item model
        if (vis != null) Destroy(vis);
        vis = Instantiate(data.prefab, transform.position, transform.rotation, null);  // Attach model as a child
        vis.transform.position = transform.position;
        vis.transform.rotation = transform.rotation;
    }
    private void Update()
    {
        transform.position = vis.transform.position;
        transform.rotation = vis.transform.rotation;
    }
    // Called when player interacts with the item
    public void Interact()
    {
        var inv = GameManager.instance.player.GetComponent<Inventory>();

        if (inv) inv.TryAdd(data);  // Add item to player's inventory
        Destroy(vis);
        Destroy(gameObject);        // Remove pickup from the world
    }
}