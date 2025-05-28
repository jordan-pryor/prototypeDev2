using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hound : MonoBehaviour
{
    enum DefaultBehavior { Patrol, WanderFixed, WanderUnfixed }
    enum AIState { Default, Chasing, Searching, Melee, Alert }
    enum StatScore { S, A, B, C, D, E, F }

    [Header("Scene refs")]
    public SphereCollider sphere;            // detection trigger
    [SerializeField] Transform eyes;         // raycast origin
    [SerializeField] LayerMask visionBlockMask;
    [SerializeField] GameObject barkSoundObject;

    [Header("AI Settings")]
    [SerializeField] DefaultBehavior defBehavior;
    [SerializeField] AIState currentState = AIState.Default;

    [Header("Patrol / Wander")]
    public List<Vector3> patrolPoints = new();
    public float wanderRadius = 10f;
    public float waitTimeAtPoint = 2f;

    [Header("Ranges / Scores")]
    [SerializeField] float meleeRange = 0.5f;
    [SerializeField] StatScore HPScore = StatScore.D;
    [SerializeField] StatScore damageScore = StatScore.C;
    [SerializeField] StatScore speedScore = StatScore.S;
    [SerializeField] StatScore sightRangeScore = StatScore.F;
    [SerializeField] StatScore visionScore = StatScore.F;
    [SerializeField] StatScore smellRangeScore = StatScore.S;
    [SerializeField] StatScore sensitivityScore = StatScore.S;
    [SerializeField] StatScore hearingRangeScore = StatScore.C;

    [Header("Timers")]
    [SerializeField] float giveUpTime = 2f;   // chase grace period

    NavMeshAgent agent;
    Animator anim;
    Transform player;

    Vector3 origin;
    Vector3 targetPoint;          // sound destination
    Vector3 lastHeardPos;

    float sightRange, smellRange, vision, sensitivity, hearRange;
    float damage, speed;

    float lostSightTimer;
    int patrolIndex;
    float waitTimer;
    bool waiting;
    bool playerInTrigger;
    bool heardSound;
    bool chasingPlayer;          // true = chase player, false = chase sound
    public bool isInSpawn = false;
    public bool inTitle = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        origin = transform.position;
        if (!inTitle)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        UpdateStats();
        if (!isInSpawn && !inTitle) GameManager.instance.updateGameGoal(1);
    }
    void Update()
    {
        CheckSensors();        // sight, sound, smell, path status
        UpdateStateLogic();    // per-state behaviour
        UpdateFacing();
        UpdateAnimator();
    }
    void CheckSensors()
    {
        if (heardSound)
        {
            heardSound = false;
            chasingPlayer = false;
            targetPoint = lastHeardPos;
            currentState = AIState.Chasing; // reuse chase code
            return;
        }
        if (playerInTrigger && CanSeePlayer())
        {
            chasingPlayer = true;
            lostSightTimer = 0f;
            currentState = AIState.Chasing;
        }
        if(!inTitle)
        {
            if (currentState != AIState.Chasing && GameManager.instance.playerController.smell >= sensitivity)
            {
                currentState = AIState.Searching;
            }
        }
        if (currentState == AIState.Chasing &&
            agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            agent.ResetPath();
            chasingPlayer = false;
            currentState = AIState.Alert;
        }
    }
    bool CanSeePlayer()
    {
        Vector3 dir = (player.position - eyes.position).normalized;
        float dist = Vector3.Distance(eyes.position, player.position);
        bool clear = !Physics.Raycast(eyes.position, dir, dist, visionBlockMask);
        bool stealthOK = false;
        if (!inTitle)
        {
            stealthOK = GameManager.instance.playerController.currentStealth <= vision;
        }
        return clear && stealthOK;
    }
    void UpdateStateLogic()
    {
        switch (currentState)
        {
            case AIState.Default: RunDefault(); break;
            case AIState.Chasing: Chase(); break;
            case AIState.Searching: HandleSearch(); break;
            case AIState.Melee: Melee(); break;
            case AIState.Alert: Bark(); break;
        }
    }
    void RunDefault()
    {
        switch (defBehavior)
        {
            case DefaultBehavior.Patrol: Patrol(); break;
            case DefaultBehavior.WanderFixed: Wander(origin); break;
            case DefaultBehavior.WanderUnfixed: Wander(transform.position); break;
        }
    }
    void Patrol()
    {
        agent.speed = speed / 2;
        if (patrolPoints.Count == 0) { AddPatrolPoints(2, origin); return; }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (!waiting) { waitTimer = waitTimeAtPoint; waiting = true; }
            else
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    patrolIndex = (patrolIndex + 1) % patrolPoints.Count;
                    agent.SetDestination(patrolPoints[patrolIndex]);
                    waiting = false;
                }
            }
        }
    }
    void Wander(Vector3 center)
    {
        agent.speed = speed / 2;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (patrolPoints.Count == 0) AddPatrolPoints(1, center);
            if (patrolPoints.Count > 0)
            {
                agent.SetDestination(patrolPoints[0]);
                patrolPoints.RemoveAt(0);
            }
        }
    }
    void AddPatrolPoints(int count, Vector3 center)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 rnd = center + new Vector3(
                Random.Range(-wanderRadius, wanderRadius), 0f,
                Random.Range(-wanderRadius, wanderRadius));

            if (NavMesh.SamplePosition(rnd, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                patrolPoints.Add(hit.position);
        }
    }
    void Chase()
    {
        agent.speed = speed;

        if (chasingPlayer)                  // chasing the player
        {
            agent.SetDestination(player.position);

            if (!CanSeePlayer())             // lost LOS
            {
                lostSightTimer += Time.deltaTime;
                if (lostSightTimer >= giveUpTime)
                {
                    chasingPlayer = false;
                    currentState = AIState.Searching;
                    agent.ResetPath();
                    return;
                }
            }
            else lostSightTimer = 0f;

            if (Vector3.Distance(transform.position, player.position) <= meleeRange)
                currentState = AIState.Melee;
        }
        else                                 // chasing a sound
        {
            agent.SetDestination(targetPoint);

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
                currentState = AIState.Searching;
        }
    }
    void HandleSearch()
    {
        origin = transform.position;
        AddPatrolPoints(1, origin);
        currentState = AIState.Default;
    }
    void Bark()
    {
        agent.speed = 0f;
    }
    public void MeleeAttack()                // called by anim event
    {
        if (Vector3.Distance(transform.position, player.position) <= meleeRange)
            player.GetComponent<IDamage>()?.TakeDamage(damage);

        currentState = AIState.Chasing;
    }
    void Melee() { /**/ }
    void FaceTarget(Vector3 dest)
    {
        Vector3 dir = (dest - transform.position).normalized; dir.y = 0f;
        if (dir == Vector3.zero) return;
        transform.rotation = Quaternion.Slerp(
            transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
    }
    void UpdateFacing()
    {
        if (agent.hasPath && agent.remainingDistance > 0.1f)
            FaceTarget(agent.destination);
    }
    void UpdateAnimator()
    {
        float v = agent.velocity.magnitude;
        anim.SetBool("isMoving", v > 0.1f);
        anim.SetFloat("Speed", currentState == AIState.Chasing ? 1f : 0f);
        anim.SetBool("isSearching", currentState == AIState.Searching);
        anim.SetBool("isAlerting", currentState == AIState.Alert);
        anim.SetBool("isAttacking", currentState == AIState.Melee);
    }
    void UpdateStats()
    {
        float r = sphere.radius;
        sightRange = GetStat(sightRangeScore, r);
        smellRange = GetStat(smellRangeScore, r);
        hearRange = GetStat(hearingRangeScore, r);
        vision = GetStat(visionScore, 100f);
        sensitivity = GetStat(sensitivityScore, 100f);
        damage = GetStat(damageScore, 50f);
        speed = GetStat(speedScore, 10f);
        agent.speed = speed;
    }
    static float GetStat(StatScore s, float baseVal) => s switch
    {
        StatScore.S => baseVal * 1.0f,
        StatScore.A => baseVal * 0.85f,
        StatScore.B => baseVal * 0.65f,
        StatScore.C => baseVal * 0.50f,
        StatScore.D => baseVal * 0.40f,
        StatScore.E => baseVal * 0.20f,
        _ => baseVal * 0.10f
    };
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInTrigger = true;
        if (other.CompareTag("Sound"))
        {
            heardSound = true;
            lastHeardPos = other.transform.position;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInTrigger = false;
    }
    public void SpawnSound()                 // called by bark anim event
    {
        if (barkSoundObject) Instantiate(barkSoundObject, transform.position, Quaternion.identity);
        currentState = AIState.Searching;
    }
               
}