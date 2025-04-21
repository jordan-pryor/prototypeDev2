using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IInteract
{
    [SerializeField] GameObject door;
    [SerializeField, Range(0f, 1f)] float openPercent;
    [SerializeField] GameObject doorMin;
    [SerializeField] GameObject doorMax;
    [SerializeField] float openSpeed = 1f;
    [SerializeField] bool isLocked = false;

    Coroutine currentRoutine;

    void Start()
    {
        // Start State + Hide the door range refs
        doorMin.SetActive(false);
        doorMax.SetActive(false);
        ApplyRotation();
    }
    void OnValidate()
    {
        // Update in Editor
        ApplyRotation();
    }
    void ApplyRotation()
    {
        // Clamps and Updates rotation to percentage of the range
        openPercent = Mathf.Clamp01(openPercent);
        door.transform.rotation = Quaternion.Lerp(doorMin.transform.rotation, doorMax.transform.rotation, openPercent);
    }
    IEnumerator AnimateDoor(float target)
    {
        // Rotates the door till open/close position
        while(!Mathf.Approximately(openPercent, target))
        {
            openPercent = Mathf.MoveTowards(openPercent, target, Time.deltaTime * openSpeed);
            ApplyRotation();
            yield return null;
        }
        openPercent = target;
        ApplyRotation();
    }
    public void Open()
    {
        // Start open routine
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(AnimateDoor(1f));
    }
    public void Close()
    {
        // Start close routine
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(AnimateDoor(0f));
    }
    public void Interact()
    {
        // On Interact if not locked open or close
        if (isLocked) return; // lock code goes here
        if (openPercent < 0.01f) Open();
        else Close();
    }
}

