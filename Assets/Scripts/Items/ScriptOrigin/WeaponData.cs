using UnityEngine;

[CreateAssetMenu( menuName = "Items/Weapon")]
public class WeaponData : BaseData
{
    public int maxAmmo = 30;              // Maximum ammo capacity
    public float fireRate = 0.1f;         // Time between shots
    public float damage = 10f;            // Damage per shot
    public Sound shotSound;               // Sound to play when firing
    public float reloadTime = 1.5f;       // Time it takes to reload
    public float range = 50f;             // Max shooting range
    public GameObject hitFX;              // Visual effect on hit
    public Sound reloadFX;                // Sound to play when reloading
}
