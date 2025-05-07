using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteract
{
    public BaseData data;        // Item data (can be weapon or general object)
    private GameObject vis;      // Visual representation of the item

    private void Start()
    {
        // Remove any existing visuals and instantiate the item model
        if (vis != null) Destroy(vis);
        vis = Instantiate(data.prefab, transform);  // Attach model as a child
    }

    // Called when player interacts with the item
    public void Interact()
    {
        var inv = GameManager.instance.player.GetComponent<Inventory>();

        if (inv) inv.TryAdd(data);  // Add item to player's inventory
        Destroy(gameObject);        // Remove pickup from the world
    }
}