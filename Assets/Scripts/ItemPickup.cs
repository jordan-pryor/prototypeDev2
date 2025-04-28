using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteract
{
    public ItemData data;
    private GameObject vis;
    private void Start()
    {
        if (vis != null) Destroy(vis);
        vis = Instantiate(data.prefab, transform);
    }
    public void Interact()
    {
        var inv = GameManager.instance.player.GetComponent<Inventory>();
        if (inv) inv.TryAdd(data);
        Destroy(gameObject);
    }
}