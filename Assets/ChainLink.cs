using UnityEngine;
using System;

public class ChainLink : MonoBehaviour
{
    // raised when this link touches the player
    public static event Action<ChainLink> OnAnyLinkHit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) OnAnyLinkHit?.Invoke(this);
    }
}