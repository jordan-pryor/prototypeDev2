using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform[] defaultPoints;
    public List<Transform> targetPoints = new List<Transform>();
    public enum Behavior { Default, Move, Search, Action }
    public Behavior currentBehavior = Behavior.Default;
    [SerializeField] private EnemyBehavior defaultBehavior;
    [SerializeField] private EnemyBehavior moveBehavior;
    [SerializeField] private EnemyBehavior searchBehavior;
    [SerializeField] private EnemyBehavior actionBehavior;
    public bool seenPlayer = false;
    //private Animator animator;
    public Animator animator;
    private IEnemy currentScript;
    public NavMeshAgent agent;
    public float guardDistance = 5f;
    public float memoryDuration = 3f;
    public float memoryTimer = 0f;
    [SerializeField] GameObject soundObject;
    void Update()
    {
        /*
        switch (currentBehavior)
        {
            case Behavior.Move:
                animator.SetInteger("State", 3); break;
            case Behavior.Search:
                animator.SetInteger("State", 1); break;
            case Behavior.Action:
                animator.SetInteger("State", 2); break;
            default:
                animator.SetInteger("State", 0); break;
        }
        */
        currentScript = GetCurrentScript();
        if (currentScript != null)
        {
            currentScript.Execute(this);
        }
        if (!seenPlayer)
        {
            memoryTimer -= Time.deltaTime;
        }
    }

    public void OnSightTrigger(SenseTrigger source, Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetBehavior(Behavior.Move, other.transform);
            memoryTimer = memoryDuration;
            seenPlayer = true;
        }
    }

    public void OnSightExit(SenseTrigger source, Collider other)
    {
        if (other.CompareTag("Player"))
        {
            seenPlayer = false;
        }
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

    public void OnActionTrigger(SenseTrigger source, Collider other)
    {
        if (other.CompareTag("Player") && seenPlayer)
        {
            SetBehavior(Behavior.Action);
        }
    }

    public void SetBehavior(Behavior newBehavior, Transform target = null)
    {
        currentBehavior = newBehavior;
        if (target != null && !targetPoints.Contains(target))
        {
            targetPoints.Add(target);
        }
    }
    public void OnAnimationEnd()
    {
        animator.SetBool("isSearching", false);
        animator.SetBool("isActing", false);
        SetBehavior(Behavior.Default);
    }
    public void onAlertEnd()
    {
        Instantiate(soundObject, transform.position, transform.rotation);
        animator.SetBool("isActing", false);
    }

    private IEnemy GetCurrentScript()
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
