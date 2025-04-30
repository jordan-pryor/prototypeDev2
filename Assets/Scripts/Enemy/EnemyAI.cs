using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] Transform attackPos;
    [SerializeField] GameObject rangedProjectile;
    [SerializeField] GameObject meleeProjectile;
    [SerializeField] float attackRate;

    float attackTimer;

    Color colorOrig;

    Vector3 playerDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created      

    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            faceTarget();
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackRate)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
                meleeAttack();
            else
                rangedAttack();
        }

        playerDirection = (GameManager.instance.player.transform.position - transform.position);
        agent.SetDestination(GameManager.instance.player.transform.position);
    }

    IEnumerator flashRed()
    {
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    void rangedAttack()
    {
        attackTimer = 0;
        Instantiate(rangedProjectile, attackPos.position, transform.rotation);
    }

    void meleeAttack()
    {
        attackTimer = 0;
        Instantiate(meleeProjectile, attackPos.position, transform.rotation);
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, transform.position.y, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void IDamage.TakeDamage(float amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            Destroy(gameObject);
            GameManager.instance.updateGameGoal(-1);            
        }
    }
}
