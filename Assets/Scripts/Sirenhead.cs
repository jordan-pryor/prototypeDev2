﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sirenhead : MonoBehaviour, IDamage
{
    enum DefaultBehavior { Patrol, WanderFixed, WanderUnfixed, Guard }
    enum AIState { Default, Chasing, Searching, Alert }
    enum StatScore { S, A, B, C, D, E, F }

    [Header("Scene refs")]
    public SphereCollider sphere;            // detection trigger
    [SerializeField] Transform eyes;         // raycast origin
    [SerializeField] LayerMask visionBlockMask;
    [SerializeField] GameObject hornSoundObject;

    [Header("AI Settings")]
    [SerializeField] DefaultBehavior defBehavior;
    [SerializeField] AIState currentState = AIState.Default;

    [Header("Patrol / Wander")]
    public List<Vector3> patrolPoints = new();
    public float wanderRadius = 10f;
    public float waitTimeAtPoint = 2f;

    [Header("Ranges / Scores")]
    [SerializeField] float screamRange = 5f;
    [SerializeField] StatScore HPScore = StatScore.D;
    [SerializeField] StatScore damageScore = StatScore.C;
    [SerializeField] StatScore speedScore = StatScore.S;
    [SerializeField] StatScore sightRangeScore = StatScore.F;
    [SerializeField] StatScore visionScore = StatScore.F;
    [SerializeField] StatScore smellRangeScore = StatScore.S;
    [SerializeField] StatScore sensitivityScore = StatScore.S;
    [SerializeField] StatScore hearingRangeScore = StatScore.C;
    float viewAngle = 180f;
    float maxHP = 100;
    float HP = 10;
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
    public Sound footsteps;
    float lostSightTimer;
    int patrolIndex;
    float waitTimer;
    bool waiting;
    bool playerInTrigger;
    bool heardSound;
    bool chasingPlayer;          // true = chase player, false = chase sound
    public bool isInSpawn = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        origin = transform.position;
        if (patrolPoints.Count < 1) patrolPoints.Add(origin);
        UpdateStats();
        if (!isInSpawn) GameManager.instance.updateGameGoal(1);
    }
    void Update()
    {
        CheckSensors();        // sight, sound, smell, path status
        UpdateStateLogic();    // per-state behaviour
        UpdateFacing();
        UpdateAnimator();
    }
    public void Step()
    {
        Instantiate(footsteps, transform.position, transform.rotation); // Play footstep sound
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
        if (currentState != AIState.Chasing &&
            GameManager.instance.playerController.smell >= sensitivity)
        {
            currentState = AIState.Searching;
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
        Vector3 toPlayer = player.position - eyes.position;
        float dist = toPlayer.magnitude;
        Vector3 dir = toPlayer / dist;
        float halfFOV = viewAngle * 0.5f;
        if (Vector3.Angle(eyes.forward, dir) > halfFOV)
            return false;
        if (Physics.Raycast(eyes.position, dir, dist, visionBlockMask))
            return false;
        if (GameManager.instance.playerController.currentStealth > vision)
            return false;
        return true;
    }
    void UpdateStateLogic()
    {
        switch (currentState)
        {
            case AIState.Default: RunDefault(); break;
            case AIState.Chasing: Chase(); break;
            case AIState.Searching: HandleSearch(); break;
            case AIState.Alert: Scream(); break;
        }
    }
    void RunDefault()
    {
        switch (defBehavior)
        {
            case DefaultBehavior.Patrol: Patrol(); break;
            case DefaultBehavior.WanderFixed: Wander(origin); break;
            case DefaultBehavior.WanderUnfixed: Wander(transform.position); break;
            case DefaultBehavior.Guard: Guard(); break;
        }
    }
    void Guard()
    {
        agent.speed = 0f;
        // Ensure we have a guard point
        if (patrolPoints.Count == 0) AddPatrolPoints(1, origin);
        Vector3 guardPoint = patrolPoints[0];
        float distance = Vector3.Distance(transform.position, guardPoint);
        // If too far from guard point, walk back to it
        if (distance > 1f)
        {
            agent.speed = speed;
            agent.SetDestination(guardPoint);
            return;
        }
        // At position just idle here, wait for stimuli
        agent.ResetPath();
    }
    void Patrol()
    {
        agent.speed = speed;
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
        agent.speed = speed;
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
            Vector2 rnd2D = Random.insideUnitCircle.normalized;
            Vector3 dir = new Vector3(rnd2D.x, 0f, rnd2D.y);
            if (Physics.Raycast(center, dir, out RaycastHit wallHit, wanderRadius, visionBlockMask))
            {
                float t = Random.Range(0f, wallHit.distance);
                Vector3 samplePoint = center + dir * t;
                if (NavMesh.SamplePosition(samplePoint, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                {
                    patrolPoints.Add(navHit.position);
                }
            }
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
            if (Vector3.Distance(transform.position, player.position) <= screamRange)
                currentState = AIState.Alert;
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
        agent.speed = 0;
        origin = transform.position;
        AddPatrolPoints(1, origin);
        currentState = AIState.Default;
    }
    void Scream()
    {
        agent.speed = 0f;
        agent.ResetPath();
    }
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
        anim.SetBool("isWalking", v > 0.1f);
        anim.SetBool("isSearching", currentState == AIState.Searching);
        anim.SetBool("isActing", currentState == AIState.Alert);
    }
    void UpdateStats()
    {
        float r = sphere.radius;
        sightRange = GetStat(sightRangeScore, r);
        smellRange = GetStat(smellRangeScore, r);
        hearRange = GetStat(hearingRangeScore, r);
        vision = GetStat(visionScore, 150f);
        sensitivity = (1f / GetStat(sensitivityScore, 1f)) * 10f;
        damage = GetStat(damageScore, 50f);
        speed = GetStat(speedScore, 10f);
        agent.speed = speed;
        viewAngle = GetStat(sightRangeScore, 180f);
        maxHP = GetStat(HPScore, 100);
        HP = maxHP;
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
        if (other.CompareTag("Sound"))
        {
            Vector3 soundPos = other.transform.position;
            if (Vector3.Distance(transform.position, soundPos) <= hearRange)
            {
                heardSound = true;
                lastHeardPos = soundPos;
            }
        }
        if (other.CompareTag("Player"))
            playerInTrigger = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInTrigger = false;
    }
    public void SpawnSound()                 // called by scream anim event
    {
        if (hornSoundObject) Instantiate(hornSoundObject, transform.position, Quaternion.identity);
        if (playerInTrigger)
        {
            if (Vector3.Distance(transform.position, player.position) <= screamRange)
                currentState = AIState.Alert;
            else
            {
                currentState = AIState.Chasing;
            }
        }
        else
        {
            currentState = AIState.Searching;
        }
    }
    public void TakeDamage(float amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }
}