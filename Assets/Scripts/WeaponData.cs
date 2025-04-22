using UnityEngine;

[CreateAssetMenu( menuName = "Items/Weapon")]
public class WeaponData : ItemData
{
    public int maxAmmo = 30;
    public float fireRate = 0.1f;
    public float damage = 10f;
    public Sound shotSound;
}
