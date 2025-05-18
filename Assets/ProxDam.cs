using UnityEngine;

public class ProxDam : MonoBehaviour
{
    [SerializeField] SphereCollider sc;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamage dmg)) dmg.TakeDamage(1.0f);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out IDamage dmg)) dmg.TakeDamage(0.5f);
    }
}
