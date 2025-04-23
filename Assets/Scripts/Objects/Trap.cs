using UnityEngine;
using System.Collections;

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
