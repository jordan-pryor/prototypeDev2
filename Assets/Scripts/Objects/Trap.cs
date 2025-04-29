using UnityEngine;
using System.Collections;
using NUnit;

public class Trap : MonoBehaviour
{
    enum TrapType { NoReset, Auto, Manual }
    enum TrapState { Active, Inactive, Reset };
    [Header("Trap Options")]
    [SerializeField] TrapType type = TrapType.Auto;
    [SerializeField] float speedDecrease = 0.5f;
    [SerializeField] int trapDurationSeconds = 5;
    [SerializeField] int trapResetSeconds = 2;

    TrapState state = TrapState.Active;
    WaitForSeconds resetWait;
    private void Start()
    {
        resetWait = new WaitForSeconds(trapResetSeconds);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || state != TrapState.Active) return;
        if (other.TryGetComponent<ITrap>(out var victim)) StartCoroutine(victim.trap(speedDecrease, trapDurationSeconds));
        if(type != TrapType.NoReset)
        {
            state = TrapState.Inactive;
            if (type == TrapType.Auto) StartCoroutine(ResetTrap());
        }
    }
    IEnumerator ResetTrap()
    {
        state = TrapState.Reset; // reset
        yield return resetWait;
        state = TrapState.Active; // active
    }
}

/*
 * was on player controller
 * === if we can crouch, then instantiate, then uncrouch easily, that would add to the experience. if not, all good
*
void placeTrap(Trap trap)
{
    // place trap at player pos
    Vector3 trapPos = transform.position;
    // change y to 0
    trapPos.y = 0;
    // crouch

    // instantiate trap.trap??
    Instantiate(trap.trapToSet, trapPos, Quaternion.identity);
    // uncrouch

    // empty hands
    itemModel.GetComponent<MeshFilter>().sharedMesh = null;
    itemModel.GetComponent<MeshRenderer>().sharedMaterial = null;
    // remove from inv
    inv.Remove(inv[invPos]);
    invPos = inv.Count - 1;
}
*/
