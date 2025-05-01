using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour, IUse
{
    int maxAmmo;
    int currentAmmo;
    float fireRate;
    float damage;
    float nextShot;
    float reloadTime;
    bool isReloading;
    float range;
    GameObject hitFX;
    Sound sfx;
    Sound rfx;
    public void PullStat(WeaponData data)
    {
        maxAmmo = data.maxAmmo;
        currentAmmo = data.maxAmmo;
        fireRate = data.fireRate;
        damage = data.damage;
        reloadTime = data.reloadTime;
        range = data.range;
        hitFX = data.hitFX;
        sfx = data.shotSound;
        rfx = data.reloadFX;
    }
    public void Use(bool primary)
    {
        if (primary)
        {
            TryShoot();
        }
        else
        {
            TryReload();
        }
    }
    private void TryShoot()
    {
        if (isReloading || Time.time < nextShot) return;
        if(currentAmmo > 0)
        {
            currentAmmo--;
            nextShot = Time.time + fireRate;
            Shoot();
        }
        else
        {
            TryReload();
        }
    }
    private void TryReload()
    {
        if (isReloading || currentAmmo == maxAmmo) return;
        StartCoroutine(Reload());
    }
    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        Instantiate(rfx, transform.position, transform.rotation);
        isReloading = false;
    }
    private void Shoot()
    {
        Camera cam = Camera.main;
        Vector3 origin = cam.transform.position;
        Vector3 dir = cam.transform.forward;
        Instantiate(sfx, transform.position, transform.rotation);
        if (Physics.Raycast(origin, dir, out RaycastHit hit, range))
        {
            if (hit.collider.TryGetComponent(out IDamage target))
            {
                target.TakeDamage(damage);
                GameObject fx = Instantiate(hitFX, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(fx, 1f);
            }
        }
    }
}
