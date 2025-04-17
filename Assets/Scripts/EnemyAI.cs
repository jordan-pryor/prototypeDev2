using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;

    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;
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
<<<<<<< Updated upstream
        //colorOrig = model.material.color;
=======
        colorOrig = model.material.color;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
            if(agent.remainingDistance <= agent.stoppingDistance){
                meleeAttack();
             }else{
=======
            rangedAttack();
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                meleeAttack();
            }
            else
            {
>>>>>>> Stashed changes
                rangedAttack();
            }
        }

        playerDirection = (GameManager.instance.player.transform.position - transform.position);
        agent.SetDestination(GameManager.instance.player.transform.position);
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            Destroy(gameObject);
            GameManager.instance.updateGameGoal(-1);
        }
    }

    IEnumerator flashRed()
    {
<<<<<<< Updated upstream
        //model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        //model.material.color = colorOrig;
=======
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
>>>>>>> Stashed changes
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
}
