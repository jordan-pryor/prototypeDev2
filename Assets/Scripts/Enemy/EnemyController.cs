using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour, IDamage, ITrap
{
    // AI behavior states and patrol path types
    public enum Behavior { Default, Move, Search, Action }
    public enum PathMode { Cycle, PingPong, Random }

    [Header("References")]
    [SerializeField] private EnemyBehavior defaultBehavior;     // Idle/guard behavior
    [SerializeField] private EnemyBehavior moveBehavior;        // Chase behavior
    [SerializeField] private EnemyBehavior searchBehavior;      // Search behavior
    [SerializeField] private EnemyBehavior actionBehavior;      // Attack behavior
    public Animator animator;
    public NavMeshAgent agent;
    [SerializeField] GameObject soundObject;                    // Used to alert others or make noise
    public List<Transform> defaultPoints = new();               // Patrol or guard points
    public List<Transform> targetPoints = new();                // Interest points (player, etc.)
    public Vector3 origin;
    public LayerMask wallMask;
    public GameObject bullet;
    public Transform attackPos;

    [Header("State")]
    public Behavior currentBehavior = Behavior.Default;
    public bool seenPlayer = false;
    public float memoryTimer = 0f;
    private IEnemy currentScript;

    [Header("Vision")]
    public Transform[] eyes;
    public float sightRange = 20f;
    public float sightFOV = 90f;
    public LayerMask sightMask;

    [Header("Action Range")]
    public Transform[] actionOrigins;
    public float actionRange = 20f;
    public float actionFOV = 90f;
    public LayerMask actionMask;

    [Header("Stats")]
    public float guardDistance = 5f;
    public float memoryDuration = 3f;
    public float searchTime = 5f;
    public float turnSpeed = 5f;
    public int detection = 50;
    public float speed = 3.5f;
    private float HP = 5f;
    public float maxHP = 5f;
    public int patrolIndex = 0;
    public int patrolDir = 1;
    public int patrolPoints = 5;
    public PathMode pathMode = PathMode.Cycle;
    public float patrolWait = 1f;
    public float damage = 2;

    //Stun Settings
    bool isStunned;
    Coroutine stunCourtine;
    float trapDecrease = 0f;


    private void Start()
    {
        agent.speed = speed;               // Set movement speed
        origin = transform.position;       // Save original spawn position
        if (agent != null) agent.speed = speed;  //Set stun check.
    }

    void Update()
    {
        if(isStunned)
        {
            if (agent != null) agent.isStopped = true;
            return;
        }

        if (seenPlayer) FacePlayer();      // Rotate toward player if seen
        memoryTimer -= seenPlayer ? 0f : Time.deltaTime;  // Countdown memory if player not seen
        currentScript = GetCurrentBehavior();              // Get behavior script
        currentScript.Execute(this);                       // Run behavior

        if(agent != null)
        {
            agent.isStopped = false;
            agent.speed = speed;
        }

        
    }

    // Called by ray-based enemy vision
    public void RayTrigger(Collider other)
    {
        if (currentBehavior == Behavior.Action) return;
        memoryTimer = memoryDuration;
        SetBehavior(Behavior.Move, other.transform);  // Start chasing
    }

    // Triggered by hearing sound
    public void OnHearingTrigger(SenseTrigger source, Collider other)
    {
        if (other.TryGetComponent<Sound>(out Sound sound))
        {
            SetBehavior(Behavior.Move, other.transform);  // Investigate noise
        }
    }

    // Triggered by smell detection
    public void OnSmellTrigger(SenseTrigger source, Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetBehavior(Behavior.Search);  // Begin searching area
        }
    }

    // Change behavior and optionally add a target
    public void SetBehavior(Behavior newBehavior, Transform target = null)
    {
        currentBehavior = newBehavior;
        currentScript = GetCurrentBehavior();
        if (target != null && !targetPoints.Contains(target))
        {
            targetPoints.Add(target);
        }
    }

    // Called when search behavior finishes
    public void OnSearchEnd()
    {
        SetBehavior(seenPlayer ? Behavior.Move : Behavior.Default);
    }

    // Called when alert animation finishes
    public void OnAlertEnd()
    {
        Instantiate(soundObject, transform.position, transform.rotation);  // Emit noise
        SetBehavior(seenPlayer ? Behavior.Move : Behavior.Default);
    }

    // Called at end of action animation
    public void OnAnimationEnd()
    {
        Instantiate(soundObject, transform.position, transform.rotation);  // Emit noise
        SetBehavior(seenPlayer ? Behavior.Move : Behavior.Default);
    }

    // Smoothly rotate toward the player
    public void FacePlayer()
    {
        Vector3 dir = GameManager.instance.player.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * turnSpeed);
        }
    }

    // Returns behavior script based on current state
    private IEnemy GetCurrentBehavior()
    {
        return currentBehavior switch
        {
            Behavior.Move => moveBehavior as IEnemy,
            Behavior.Search => searchBehavior as IEnemy,
            Behavior.Action => actionBehavior as IEnemy,
            _ => defaultBehavior as IEnemy
        };
    }

    // Apply damage and destroy enemy if HP is 0
    void IDamage.TakeDamage(float amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Destroy(gameObject);
            GameManager.instance.updateGameGoal(-1);  // Notify manager on death
        }
    }

    // Finds a wall direction and places a point between origin and hit
    public Transform TowardWall(float pMin, float pMax)
    {
        Vector2 ran = Random.insideUnitCircle.normalized;
        Vector3 dir = new Vector3(ran.x, 0f, ran.y);
        Vector3 pos;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, Mathf.Infinity, wallMask))
        {
            pos = Vector3.Lerp(origin, hit.point, Random.Range(pMin, pMax));
        }
        else
        {
            pos = origin;
        }

        GameObject obj = new GameObject(name);  // Creates a dummy point object
        obj.transform.position = pos;
        return obj.transform;
    }

    // Fires a projectile forward from the attack position
    public void Fire()
    {
        GameObject bull = Instantiate(bullet, attackPos.position, attackPos.rotation);
        if (bull != null && bull.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(attackPos.forward * 5, ForceMode.Impulse);
        }
    }

    // Deals melee damage if player is in range
    public void Melee()
    {
        Transform playerPos = GameManager.instance.player.transform;
        if (Vector3.Distance(transform.position, playerPos.position) < actionRange)
        {
            if (playerPos.TryGetComponent(out IDamage dmg)) dmg.TakeDamage(damage);
        }
    }

    //Commented out for time may be needed later. 
    //public void Stun(float duration)
    //{
    //    if(stunCourtine != null)
    //    {
    //        StopCoroutine(stunCourtine);
    //    }

    //    stunCourtine = StartCoroutine(StunRoutine(duration));
    //}

    //IEnumerator StunRoutine(float duration)
    //{
    //    isStunned = true;
    //    yield return new WaitForSeconds(duration);
    //    isStunned = false;
    //    stunCourtine = null;

    //    if(agent != null)
    //    {
    //        agent.isStopped = false;
    //        agent.speed = speed;
    //    }
    //}

    public IEnumerator trap(float speedDecrease, float duration)
    {
        if (isStunned) yield break;
        isStunned = true;
        trapDecrease = speedDecrease;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }
}