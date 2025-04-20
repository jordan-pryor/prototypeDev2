using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public List<Transform> points = new List<Transform>();
    public enum Behavior { Default, Move, Search, Action }
    public Behavior currentBehavior = Behavior.Default;
    public MonoBehaviour defaultBehavior;
    public MonoBehaviour moveBehavior;
    public MonoBehaviour searchBehavior;
    public MonoBehaviour actionBehavior;
    public bool seenPlayer = false;
    private IEnemy currentScript;
    void Update()
    {
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
        if (target != null && !points.Contains(target))
        {
            points.Add(target);
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
