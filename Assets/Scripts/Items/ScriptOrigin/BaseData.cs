using UnityEngine;
[CreateAssetMenu(menuName = "Items")]
public abstract class BaseData : ScriptableObject
{
    public enum ItemSlot { Pocket, Hand }
    public string itemName;
    public GameObject prefab;
    public GameObject emptyPickupPrefab;
    public ItemSlot slot = ItemSlot.Pocket;
    public Sound dropSound;
    public Quaternion defaultRotation;
}
