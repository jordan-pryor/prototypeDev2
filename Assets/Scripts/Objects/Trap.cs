using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class Trap : MonoBehaviour
{
    TrapData tData;
    int usesLeft;
    bool isArmed = false;

    public void PullStat(TrapData data)
    {
        tData = data;
        usesLeft = data.maxUses;
        isArmed = true;
    }

    private void Awake()
    {
        //makes sure collider is set
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isArmed || usesLeft <= 0) return;
        if (!other.CompareTag("Enemy")) return;

        isArmed = false;
        usesLeft--;

        //Damage
        if(tData.damageAmount > 0 && other.TryGetComponent<IDamage>(out var damage))
        {
            damage.TakeDamage(tData.damageAmount);
        }

        //Stun
        if(tData.stunDuration > 0 && other.TryGetComponent<ITrap>(out var enemyController))
        {
            StartCoroutine(enemyController.trap(tData.speedDecrease, tData.stunDuration));
        }

        if(tData.persistent && usesLeft > 0)
        {
            StartCoroutine(Rearm());
        }
        else
        {
            StartCoroutine(ResetToPickup());
        }
    }

    IEnumerator Rearm()
    {
        yield return new WaitForSeconds(tData.resetDelay);
        isArmed = true;
    }

    IEnumerator ResetToPickup()
    {
        yield return new WaitForSeconds(tData.resetDelay);

        var pick = Instantiate(tData.emptyPickupPrefab, transform.position, Quaternion.identity);

        pick.GetComponent<ItemPickup>().data = tData;

        Destroy(gameObject);
    }
}