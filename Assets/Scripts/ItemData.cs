using UnityEngine;
[CreateAssetMenu(menuName = "Items/Generic Item")]
public abstract class ItemData : ScriptableObject
{
    public enum ItemSlot { Pocket, Hand }
	//Crafting System ItemID
	public string itemID;
	public string itemName;
    public GameObject prefab;
    public GameObject emptyPickupPrefab;
    public ItemSlot slot = ItemSlot.Pocket;
    public Sound dropSound;
}
