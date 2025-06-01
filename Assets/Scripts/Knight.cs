using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Knight : MonoBehaviour, IDamage
{
    enum DefaultBehavior { Chase }
    enum AIState { Chasing, Attack, Anim }
    enum StatScore { S, A, B, C, D, E, F }

    [Header("Scene refs")]
    public SphereCollider sphere;            // detection trigger
    [SerializeField] Transform eyes;         // raycast origin
    [SerializeField] LayerMask visionBlockMask;
    [SerializeField] GameObject hornSoundObject;

    [Header("AI Settings")]
    [SerializeField] DefaultBehavior defBehavior;
    [SerializeField] AIState currentState = AIState.Chasing;

    [Header("Patrol / Wander")]
    public List<Vector3> patrolPoints = new();
    public float wanderRadius = 10f;
    public float waitTimeAtPoint = 2f;

    [Header("Ranges / Scores")]
    [SerializeField] float longRange = 10f;
    [SerializeField] float closeRange = 5f;
    [SerializeField] StatScore HPScore = StatScore.S;
    [SerializeField] StatScore damageScore = StatScore.A;
    [SerializeField] StatScore speedScore = StatScore.F;
    [SerializeField] StatScore sightRangeScore = StatScore.F;
    [SerializeField] StatScore visionScore = StatScore.F;
    [SerializeField] StatScore smellRangeScore = StatScore.F;
    [SerializeField] StatScore sensitivityScore = StatScore.F;
    [SerializeField] StatScore hearingRangeScore = StatScore.F;
    float viewAngle = 180f;
    float maxHP = 100;
    float HP = 10;

    [Header("Chain Attack Settings")]
    [SerializeField] GameObject chainPrefab;    // your single link prefab
    [SerializeField] float linkSpacing = 0.5f;
    [SerializeField] float deployDelay = 0.02f;  // small delay between link spawns
    [SerializeField] float pullDelay = 0.1f;   // wait after links finish
    [SerializeField] float pullDuration = 0.5f;   // how long the pull takes
    [SerializeField] Transform chainOrigin;
    [SerializeField] float playerCenterOffsetY = 1.0f;

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
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        origin = transform.position;

        UpdateStats();
        if (!isInSpawn) GameManager.instance.updateGameGoal(1);
    }
    void Update()
    {
        UpdateStateLogic();    // per-state behaviour
        UpdateFacing();
    }
    void UpdateStateLogic()
    {
        switch (currentState)
        {
            case AIState.Chasing: Chase(); break;
            case AIState.Attack: Attacks(); break;
            case AIState.Anim: Wait(); break;
        }
    }
    void Wait()
    {
        agent.speed = 0;
        agent.ResetPath();
    }
    public void Melee()                // called by anim event
    {
        if (Vector3.Distance(transform.position, player.position) <= closeRange)
            player.GetComponent<IDamage>()?.TakeDamage(damage);

        currentState = AIState.Chasing;
    }
    void Attacks()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= closeRange)
        {
            anim.SetTrigger("isPrimary");
        }
        else
        {
            anim.SetTrigger("isSecondary");
        }
        currentState = AIState.Anim;
    }
    public void Throw()
    {
        StartCoroutine(ChainAttack());
    }
    void Chase()
    {
        agent.speed = speed;
        agent.SetDestination(player.position);
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= longRange)
        {
                currentState = AIState.Attack;
        }
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
    void UpdateStats()
    {
        float r = 15;
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
    public void TakeDamage(float amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }
    IEnumerator ChainAttack()
    {
        Vector3 origin = chainOrigin.position;
        Vector3 targetPos = player.position + Vector3.up * playerCenterOffsetY;
        Vector3 dir = (targetPos - origin).normalized;
        float totalDist = Vector3.Distance(origin, targetPos);
        var spawnedLinks = new List<GameObject>();
        bool hitDetected = false;
        void OnHit(ChainLink link) => hitDetected = true;
        ChainLink.OnAnyLinkHit += OnHit;
        int linkCount = Mathf.CeilToInt(totalDist / linkSpacing);
        for (int i = 1; i <= linkCount; i++)
        {
            Vector3 spawnPos = origin + dir * (linkSpacing * i);
            var linkObj = Instantiate(chainPrefab, spawnPos, Quaternion.LookRotation(dir));
            spawnedLinks.Add(linkObj);
            yield return new WaitForSeconds(deployDelay);
        }
        yield return new WaitForSeconds(pullDelay);
        float elapsed = 0f;
        Vector3 pullStartPos = player.position;
        while (elapsed < pullDuration)
        {
            float t = elapsed / pullDuration;
            foreach (var obj in spawnedLinks)
                if (obj != null)
                    obj.transform.position = Vector3.Lerp(obj.transform.position, origin, t);
            if (hitDetected)
                player.position = Vector3.Lerp(pullStartPos, origin, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (hitDetected)
            player.position = origin;
        foreach (var obj in spawnedLinks)
            if (obj != null) Destroy(obj);
        ChainLink.OnAnyLinkHit -= OnHit;
        currentState = AIState.Chasing;
    }
}