using UnityEngine;

public class KnightChainController : MonoBehaviour
{
    Transform attackPosition;

    void Start()
    {
        attackPosition = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (other.CompareTag("player")) { 
            if(GameManager.instance.player.transform.position != attackPosition.transform.position)
            {
                GameManager.instance.player.transform.position -= new Vector3(attackPosition.transform.position.x * .01f, attackPosition.transform.position.y, attackPosition.transform.position.z * .01f);
            }
        }
    }
}
