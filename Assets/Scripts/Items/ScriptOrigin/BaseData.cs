using UnityEngine;
[CreateAssetMenu(menuName = "Items")]
public abstract class BaseData : ScriptableObject
{
    public string itemName;                    // Name shown in UI or logs
    public GameObject prefab;                  // Visual model used in hand or inventory
    public GameObject emptyPickupPrefab;       // Optional pickup version when item is dropped
    public Sound dropSound;                    // Sound played when dropped
    public Quaternion defaultRotation;         // Rotation used when placed in socket/hand
}
