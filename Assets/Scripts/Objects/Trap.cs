using UnityEngine;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class Trap : MonoBehaviour
{
    float slowMulti;
    float duration;
    bool isArmed = false;

    public void PullStat(TrapData data)
    {
        slowMulti = data.slowMultiplier;
        duration = data.trapDuration;
    }

    private void Awake()
    {
        //makes sure collider is set
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnEnable()
    {
        isArmed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isArmed) return;

        if(other.CompareTag("Enemy"))
        {
            isArmed = false;

            if(other.TryGetComponent<NavMeshAgent>(out var agent))
            {
                StartCoroutine(HandleAgentSlow(agent));
            }
        }
    }

    IEnumerator HandleAgentSlow(NavMeshAgent agent)
    {
        float orginalSpeed = agent.speed;
        agent.speed = orginalSpeed * slowMulti;
        yield return new WaitForSeconds(duration);
        agent.speed = orginalSpeed;
        Destroy(gameObject, 0.1f);
    }
}