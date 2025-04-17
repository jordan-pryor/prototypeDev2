using UnityEngine;

public class Weapon : MonoBehaviour
{
	[Header("Stats")]
	[SerializeField] private int ammo = 12;
	[SerializeField] private int maxAmmo = 12;
	[SerializeField] private float fireRate = 0.5f;
	[SerializeField] private float range = 100f;
	[SerializeField] private float damage = 10f;

	[Header("References")]
	[SerializeField] private Transform shootPoint; // Where the raycast will come from

	private float nextFireTime;

	public void Shoot()
	{
		if (Time.time < nextFireTime) return;

		if (ammo <= 0)
		{
			Debug.Log("Click! Out of ammo.");
			return;
		}

		ammo--;
		nextFireTime = Time.time + fireRate;

		Debug.Log("Bang! Ammo left: " + ammo);

		// === Raycast logic ===
		if (shootPoint == null)
		{
			Debug.LogWarning("No shootPoint assigned on weapon.");
			return;
		}

		Camera cam = Camera.main;
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		if (Physics.Raycast(ray, out RaycastHit hit, range))
		{
			Debug.Log("Hit: " + hit.collider.name);
		}
		else
		{
			Debug.Log("Missed!");
		}
	}

	public void Reload()
	{
		ammo = maxAmmo;
		Debug.Log("Reloaded!");
	}
}