using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class Trap : MonoBehaviour, IUse
{
    [SerializeField] int usesLeft;
    [SerializeField] bool isArmed = false;
    [SerializeField] float damageAmount = 0f;
    [SerializeField] float stunDuration = 0f;
    [SerializeField] float speedDecrease = 0f;
    public void PullStat(TrapData data)
    {
        usesLeft = data.maxUses;
        damageAmount = data.damageAmount;
        stunDuration = data.stunDuration;
        speedDecrease = data.speedDecrease;
        isArmed = true;
    }

    public void Use(bool primary)
    {
        GameManager.instance.playerInventory.PlaceTrap(GameManager.instance.playerInventory.equipIndex);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isArmed || usesLeft <= 0) return;
        if (!(other.CompareTag("Enemy") || other.CompareTag("Player"))) return;
        //Damage
        if(damageAmount > 0 && other.TryGetComponent<IDamage>(out var damage))
        {
            damage.TakeDamage(damageAmount);
        }
        //Stun/Slow
        if(stunDuration > 0 && other.TryGetComponent<ITrap>(out var trapped))
        {
            StartCoroutine(trapped.trap(speedDecrease, stunDuration));
        }
        isArmed = false;
        usesLeft--;
    }
}