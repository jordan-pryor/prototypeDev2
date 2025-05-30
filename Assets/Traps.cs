using UnityEngine;
using System.Collections;

public class Traps : MonoBehaviour
{
    [SerializeField] float speedMultiplier = 0.5f;
    [SerializeField] float duration = 2f;
    [SerializeField] float damageAmount = 10f;
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamage>()?.TakeDamage(damageAmount);
        other.GetComponent<ITrap>()?.trapTrigger(speedMultiplier, duration);
    }
}
