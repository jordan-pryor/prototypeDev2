using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] Animator animator;
    private IEnemy currentScript;
    private void Start()
    {
        //animator = GetComponent<Animator>();
    }
    void Update()
    {
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
        currentScript = GetCurrentScript();
        currentScript.Execute(this);
    }

    public void OnSightTrigger(SenseTrigger source, Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetBehavior(Behavior.Move, other.transform);
            seenPlayer = true;
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

    private void SetBehavior(Behavior newBehavior, Transform target = null)
    {
        currentBehavior = newBehavior;
        if (target != null && !targetPoints.Contains(target))
        {
            targetPoints.Add(target);
        }
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
