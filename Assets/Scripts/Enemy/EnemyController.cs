using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour, IDamage
{
    public enum Behavior { Default, Move, Search, Action }
    public enum PathMode { Cycle, PingPong, Random }

    [Header("References")]
    [SerializeField] private EnemyBehavior defaultBehavior;
    [SerializeField] private EnemyBehavior moveBehavior;
    [SerializeField] private EnemyBehavior searchBehavior;
    [SerializeField] private EnemyBehavior actionBehavior;
    public Animator animator;
    public NavMeshAgent agent;
    [SerializeField] GameObject soundObject;
    public List<Transform> defaultPoints = new List<Transform>();
    public List<Transform> targetPoints = new List<Transform>();
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

    private void Start()
    {
        agent.speed = speed;
        origin = transform.position;
    }
    void Update()
    {
        if (seenPlayer) FacePlayer();
        memoryTimer -= seenPlayer ? 0f : Time.deltaTime;
        currentScript = GetCurrentBehavior();
        currentScript.Execute(this);
    }
    public void RayTrigger(Collider other)
    {
        if (currentBehavior == Behavior.Action) return;
        memoryTimer = memoryDuration;
        SetBehavior(Behavior.Move, other.transform);
    }
    public void OnHearingTrigger(SenseTrigger source, Collider other)
    {
        if (other.TryGetComponent<Sound>(out Sound sound))
        {
            SetBehavior(Behavior.Move, other.transform);
        }
    }
    public void OnSmellTrigger(SenseTrigger source, Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetBehavior(Behavior.Search);
        }
    }
    public void SetBehavior(Behavior newBehavior, Transform target = null)
    {
        currentBehavior = newBehavior;
        currentScript = GetCurrentBehavior();
        if (target != null && !targetPoints.Contains(target))
        {
            targetPoints.Add(target);
        }
    }
    public void OnSearchEnd()
    {
        if (seenPlayer)
        {
            SetBehavior(Behavior.Move);
        }
        else
        {
            SetBehavior(Behavior.Default);
        }
    }
    public void OnAlertEnd()
    {
        Instantiate(soundObject, transform.position, transform.rotation);
        if (seenPlayer)
        {
            SetBehavior(Behavior.Move);
        }
        else
        {
            SetBehavior(Behavior.Default);
        }
    }
    public void OnAnimationEnd()
    {
        Instantiate(soundObject, transform.position, transform.rotation);
        if (seenPlayer)
        {
            SetBehavior(Behavior.Move);
        }
        else
        {
            SetBehavior(Behavior.Default);
        }
    }
    public void FacePlayer()
    {
        Vector3 dir = GameManager.instance.player.transform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f) {
           transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * turnSpeed);
        }
    }
    private IEnemy GetCurrentBehavior()
    {
        switch (currentBehavior)
        {
            case Behavior.Move:
                return moveBehavior as IEnemy;
            case Behavior.Search:
                return searchBehavior as IEnemy;
            case Behavior.Action:
                return actionBehavior as IEnemy;
            default:
                return defaultBehavior as IEnemy;
        }
    }
    void IDamage.TakeDamage(float amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Destroy(gameObject);
            GameManager.instance.updateGameGoal(-1);
        }
    }
    public Transform TowardWall( float pMin, float pMax)
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
        GameObject obj = new GameObject(name);
        obj.transform.position = pos;
        return obj.transform;
    }
    public void Fire()
    {
        GameObject bull = Instantiate(bullet, attackPos.position, attackPos.rotation);
        if(bull != null && bull.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(attackPos.forward * 5, ForceMode.Impulse);
        }
    }
    public void Melee()
    {
        Transform playerPos = GameManager.instance.player.transform;
        if (Vector3.Distance(transform.position, playerPos.position) < actionRange)
        {
            if (playerPos.TryGetComponent(out IDamage dmg)) dmg.TakeDamage(damage);
        }
    }
}
