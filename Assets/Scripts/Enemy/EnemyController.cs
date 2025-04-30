using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public enum Behavior { Default, Move, Search, Action }
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
        SetBehavior(Behavior.Default);
    }
    public void OnAnimationEnd()
    {
        SetBehavior(Behavior.Default);
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
}
