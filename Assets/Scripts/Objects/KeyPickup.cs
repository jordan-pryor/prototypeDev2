using UnityEngine;

public class HasKey : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.instance.collectKey();
            Destroy(gameObject);

        }
    }
}
