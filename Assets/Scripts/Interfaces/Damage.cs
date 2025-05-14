using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType { moving, stationary, DOT, homing }

    [SerializeField] damageType type;           // Type of damage behavior
    [SerializeField] Rigidbody rb;              // Rigidbody for motion

    [SerializeField] float damageAmount;        // Damage dealt per hit/tick
    [SerializeField] float damageRate;          // Delay between DOT ticks
    [SerializeField] int speed;                 // Movement speed
    [SerializeField] int destroyTime;           // Lifetime for moving/homing types

    bool isDamaging;                            // Flag to prevent overlapping DOT ticks

    void Start()
    {
        // For moving or homing projectiles, destroy after a time limit
        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject, destroyTime);

            if (type == damageType.moving)
            {
                rb.linearVelocity = transform.forward * speed;  // Set initial velocity
            }
        }
    }

    void Update()
    {
        // Continuously track the player for homing type
        if (type == damageType.homing)
        {
            rb.linearVelocity =
                (GameManager.instance.player.transform.position - transform.position).normalized
                * speed * Time.deltaTime;
        }
    }

    // Deal instant damage on contact for non-DOT types
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null &&
            (type == damageType.stationary || type == damageType.moving || type == damageType.homing))
        {
            dmg.TakeDamage(damageAmount);
        }

        // Destroy projectile on hit if moving or homing
        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject);
        }
    }

    // Repeatedly deal damage over time while player stays in area
    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == damageType.DOT)
        {
            if (!isDamaging)
            {
                StartCoroutine(damageOther(dmg));
            }
        }
    }

    // Coroutine for DOT tick
    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}