using UnityEngine;
using System.Collections;
using UnityEngine.AI;

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
        Debug.Log("Trap OnTriggerEnter hit " + other.name);
        if (!isArmed || usesLeft <= 0) return;
        other.GetComponent<IDamage>()?.TakeDamage(damageAmount);
        other.GetComponent<ITrap>()?.trapTrigger(speedDecrease, stunDuration);
        isArmed = false;
        usesLeft--;
    }
}