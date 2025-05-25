using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hound : MonoBehaviour
{
    enum DefaultBehavior
    {
        Patrol,
        WanderFixed,
        WanderUnfixed
    }

    enum AIState
    {
        Default,
        Chasing,
        Searching,
        Melee,
        Alert
    }

    enum StatScore
    {
        S,
        A,
        B,
        C,
        D,
        E,
        F
    }

    public SphereCollider sphere;
    private float sphereRadius;

    [Header("AI Settings")]
    [SerializeField] DefaultBehavior defBehavior;
    [SerializeField] AIState currentState = AIState.Default;

    [Header("Wander and Patrol")]
    public List<Vector3> patrolPoints = new List<Vector3>();
    public float wanderRadius = 10f;
    public float waitTimeAtPoint = 2f;

    [Header("Sensing")]
    float sightRange;
    float hearingRange;
    float smellRange;
    [SerializeField] float meleeRange = 0.5f;
    float vision;
    float sensitivity;
    public LayerMask terrainMask;
    public LayerMask playerMask;

    [Header("Stats")]
    [SerializeField] StatScore HPScore = StatScore.D;
    [SerializeField] StatScore damageScore = StatScore.C;
    [SerializeField] StatScore speedScore = StatScore.S;
    [SerializeField] StatScore sightRangeScore = StatScore.F;
    [SerializeField] StatScore visionScore = StatScore.F;
    [SerializeField] StatScore smellRangeScore = StatScore.S;
    [SerializeField] StatScore sensitivityScore = StatScore.S;
    [SerializeField] StatScore hearingRangeScore = StatScore.C;
    float MaxHP;
    float damage;
    float speed;
    [SerializeField] private GameObject barkSoundObject;


    NavMeshAgent agent;
    Animator anim;
    Transform player;
    Vector3 origin;
    int patrolIndex = 0;
    float waitTimer = 0f;
    bool waiting = false;
    bool playerInTrigger = false;
    bool heardSound = false;
    Vector3 lastHeardPos;

    public bool isInSpawn = false;
    private bool chasingPlayer = false;
    private Vector3 targetPoint;

    void Start()
    {
        if (!isInSpawn) GameManager.instance.updateGameGoal(1);
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        origin = transform.position;
        anim = GetComponent<Animator>();
        UpdateStat();
    }

    private void UpdateStat()
    {
        sphereRadius = sphere.radius;
        sightRange = GetStat(sightRangeScore, sphereRadius);
        hearingRange = GetStat(hearingRangeScore, sphereRadius);
        smellRange = GetStat(smellRangeScore, sphereRadius);
        vision = GetStat(visionScore, 100f);
        sensitivity = GetStat(sensitivityScore, 100f);
        MaxHP = GetStat(HPScore, 200f);
        damage = GetStat(damageScore, 100f);
        speed = GetStat(speedScore, 10f);
        agent.speed = speed;
    }

    float GetStat(StatScore score, float baseRange)
    {
        return score switch
        {
            StatScore.S => baseRange * 1f,
            StatScore.A => baseRange * 0.85f,
            StatScore.B => baseRange * 0.65f,
            StatScore.C => baseRange * 0.5f,
            StatScore.D => baseRange * 0.4f,
            StatScore.E => baseRange * 0.2f,
            StatScore.F => baseRange * 0.1f,
            _ => baseRange * 0.5f
        };
    }

    void Update()
    {
        UpdateAnimator();
        UpdateFacing();
        CheckTransitions();
        switch (currentState)
        {
            case AIState.Default:
                switch (defBehavior)
                {
                    case DefaultBehavior.Patrol: Patrolling(); break;
                    case DefaultBehavior.WanderFixed: WanderFixed(); break;
                    case DefaultBehavior.WanderUnfixed: WanderUnfixed(); break;
                }
                break;
            case AIState.Chasing: ChaseTarget(); break;
            case AIState.Searching: HandleSearch(); break;
            case AIState.Melee: Melee();
                                break;
            case AIState.Alert: Bark(); break;
        }
    }

    private void Patrolling()
    {
        agent.speed = speed / 2;
        if (patrolPoints.Count == 0)
        {
            AddPatrolPoint(2, origin);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (!waiting)
            {
                waitTimer = waitTimeAtPoint;
                waiting = true;
            }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    patrolIndex = patrolIndex + 1 >= patrolPoints.Count ? 0 : patrolIndex + 1;
                    agent.SetDestination(patrolPoints[patrolIndex]);
                    waiting = false;
                }
            }
        }
    }

    private void AddPatrolPoint(int count, Vector3 center)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-wanderRadius, wanderRadius),0f,Random.Range(-wanderRadius, wanderRadius));
            Vector3 randomPoint = center + randomOffset;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                patrolPoints.Add(navHit.position);
            }
        }
    }
    private void Wander(Vector3 center)
    {
        agent.speed = speed / 2;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (patrolPoints.Count == 0) AddPatrolPoint(1, center);
            if (patrolPoints.Count > 0)
            {
                agent.SetDestination(patrolPoints[0]);
                patrolPoints.RemoveAt(0);
            }
        }
    }
    private void WanderFixed()
    {
        Wander(origin);
    }
    private void WanderUnfixed()
    {
        Wander(transform.position);
    }

    private void ChaseTarget()
    {
        agent.speed = speed;
        if (chasingPlayer)
        {
            agent.SetDestination(player.position);

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= meleeRange)
            {
                currentState = AIState.Melee;
            }
        }
        else
        {
            agent.SetDestination(targetPoint);

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                currentState = AIState.Searching;
            }
        }
    }
    private void HandleSearch()
    {
        origin = transform.position;
        AddPatrolPoint(1, origin);
        currentState = AIState.Default;
    }

    public void MeleeAttack()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= meleeRange)
        {
            IDamage target = player.GetComponent<IDamage>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
        agent.speed = speed;
        currentState = AIState.Chasing;
    }
    public void Melee()
    {
        agent.speed = 0;
    }
    private void Bark()
    {
        if (barkSoundObject != null)
        {
            Instantiate(barkSoundObject, transform.position, Quaternion.identity);
        }
        currentState = AIState.Default;
    }
    private void CheckTransitions()
    {
        if (!playerInTrigger && !heardSound) return;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (heardSound)
        {
            chasingPlayer = false;
            currentState = AIState.Chasing;
            heardSound = false;
            return;
        }
        if (GameManager.instance.playerController.currentStealth <= vision)
        {
            chasingPlayer = true;
            currentState = AIState.Chasing;
            return;
        }
        if (GameManager.instance.playerController.smell >= sensitivity)
        {
            currentState = AIState.Searching;
            return;
        }
        if (!agent.hasPath)
        {
            currentState = AIState.Alert;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
        else if (other.CompareTag("Sound"))
        {
            heardSound = true;
            targetPoint = other.transform.position;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Sound"))
        {
            playerInTrigger = false;
        }
    }
    private void FaceTarget(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }
    private void UpdateFacing()
    {
        if (agent.hasPath && agent.remainingDistance > 0.1f)
        {
            FaceTarget(agent.destination);
        }
    }
    private void UpdateAnimator()
    {
        float moveSpeed = agent.velocity.magnitude;
        anim.SetBool("isMoving", moveSpeed > 0.1f);
        float blendSpeed = (currentState == AIState.Chasing) ? 1f : 0f;
        anim.SetFloat("Speed", blendSpeed);
        anim.SetBool("isSearching", currentState == AIState.Searching);
        anim.SetBool("isAlerting", currentState == AIState.Alert);
        anim.SetBool("isAttacking", currentState == AIState.Melee);
    }
}
