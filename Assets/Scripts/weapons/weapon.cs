using UnityEngine;

public class Weapon : MonoBehaviour
{
	[Header("Stats")]
	[SerializeField] private int ammo = 12;
	[SerializeField] private int maxAmmo = 12;
	[SerializeField] private float fireRate = 1f;
	[SerializeField] private float range = 100f;
	[SerializeField] private int damage = 10;


	[Header("References")]
	[SerializeField] private Transform shootPoint; // Where the raycast will come from
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] private GameObject projectile;

    private float nextFireTime;
    float shootTimer;
    private void Update()
    {
        shootTimer += Time.deltaTime;
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * range, Color.red);
    }

    public void Shoot()
	{
        if (Time.time < nextFireTime)
            return;

        if (ammo <= 0)
        {
            Debug.Log("Click! Out of ammo.");
            return;
        }

        ammo--;
        nextFireTime = Time.time + fireRate;

        Debug.Log("Bang! Ammo left: " + ammo);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range, ~ignoreLayer))
        {
            Debug.Log("Hit: " + hit.collider.name);

            /*
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(damage);
            }
            */
            Instantiate(projectile, shootPoint.position, transform.rotation);
        }

        
    }

	public void Reload()
	{
		ammo = maxAmmo;
		Debug.Log("Reloaded!");
	}
}