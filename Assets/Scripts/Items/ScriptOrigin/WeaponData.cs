using UnityEngine;

[CreateAssetMenu( menuName = "Items/Weapon")]
public class WeaponData : BaseData
{
    public int maxAmmo = 30;
    public float fireRate = 0.1f;
    public float damage = 10f;
    public Sound shotSound;
    public float reloadTime = 1.5f;
    public float range = 50f;
    public GameObject hitFX;
    public Sound reloadFX;
}
