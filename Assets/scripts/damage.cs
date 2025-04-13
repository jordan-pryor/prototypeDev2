using System.Collections;
using UnityEngine;

public class damage : MonoBehaviour
{
    // === Types of Damage ===
    enum damageType { moving, stationary, DOT, homing }

    // === Settings (Serialized) ===
    [SerializeField] damageType type;          // What kind of pain are we dealing?
    [SerializeField] Rigidbody rb;             // Used for movement types

    [SerializeField] int damageAmount;         // How much hurt to inflict
    [SerializeField] float damageRate;         // How often for DOT
    [SerializeField] int speed;                // Movement speed (if applicable)
    [SerializeField] int destroyTime;          // Lifetime for moving/homing projectiles

    // === Internals ===
    bool isDamaging;                           // Helps throttle DOT damage

    // === Setup ===
    void Start()
    {
        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject, destroyTime);  // Auto-destroy after timer

            if (type == damageType.moving)
            {
                rb.linearVelocity = transform.forward * speed;  // Fire straight ahead
            }
        }
    }

    // === Homing Behavior ===
    void Update()
    {
        if (type == damageType.homing)
        {
            Vector3 toPlayer = (gameManager.instance.player.transform.position - transform.position).normalized;
            rb.linearVelocity = toPlayer * speed * Time.deltaTime;  // Smooth seek toward target
        }
    }

    // === On Contact: One-time Effects ===
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        iDamage dmg = other.GetComponent<iDamage>();

        if (dmg != null && (type == damageType.stationary || type == damageType.moving || type == damageType.homing))
        {
            dmg.takeDamage(damageAmount);  // Apply impact damage
        }

        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject);  // Disappear on impact
        }
    }

    // === On Contact: Continuous Damage (DOT) ===
    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        iDamage dmg = other.GetComponent<iDamage>();

        if (dmg != null && type == damageType.DOT)
        {
            if (!isDamaging)
            {
                StartCoroutine(damageOther(dmg));  // Start DOT coroutine
            }
        }
    }

    // === Coroutine for DOT ===
    IEnumerator damageOther(iDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);              // Apply tick
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
