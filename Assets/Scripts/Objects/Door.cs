using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteract
{
    [SerializeField] GameObject door;                  // The door object to rotate
    [SerializeField, Range(0f, 1f)] float openPercent; // How open the door currently is (0 = closed, 1 = open)
    [SerializeField] GameObject doorMin;               // Closed rotation reference
    [SerializeField] GameObject doorMax;               // Open rotation reference
    [SerializeField] float openSpeed = 1f;             // Speed of door animation
    public Sound doors;                                // Door sound to play

    [Header("Lock Settings")]
    [SerializeField] bool isLocked = false;            // Whether door is locked
    [SerializeField] bool isWin = false;               // Does this door trigger a win on use?
    [SerializeField] string requiredKeyName = "Key";   // Key name required to unlock door

    public bool isTransition = false;                  // Does this door load another scene?
    public int sceneIndex = -1;                        // Target scene index if transitioning

    Coroutine currentRoutine;

    void Start()
    {
        // Hide rotation references and apply initial door state
        doorMin.SetActive(false);
        doorMax.SetActive(false);
        ApplyRotation();
    }

    void OnValidate()
    {
        // Live update in editor when modifying openPercent
        ApplyRotation();
    }

    // Applies door rotation based on openPercent
    void ApplyRotation()
    {
        openPercent = Mathf.Clamp01(openPercent);
        door.transform.rotation = Quaternion.Lerp( doorMin.transform.rotation, doorMax.transform.rotation, openPercent
        );
    }

    // Animates door open/close over time
    IEnumerator AnimateDoor(float target)
    {
        while (!Mathf.Approximately(openPercent, target))
        {
            openPercent = Mathf.MoveTowards(openPercent, target, Time.deltaTime * openSpeed);
            ApplyRotation();
            yield return null;
        }
        openPercent = target;
        ApplyRotation();
    }

    // Opens the door and plays sound
    public void Open()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(AnimateDoor(1f));
        Instantiate(doors, transform.position, transform.rotation);
    }

    // Closes the door and plays sound
    public void Close()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(AnimateDoor(0f));
        Instantiate(doors, transform.position, transform.rotation);
    }

    // Main interact method (player use)
    public void Interact()
    {
        // Handle locked doors
        if (isLocked)
        {
            bool hasKey = GameManager.instance.player.GetComponent<Inventory>().Search(requiredKeyName);
            if (!hasKey)
            {
                GameManager.instance.LockPrompt();
                return;
            }
            isLocked = false; // Unlock door if player had key
        }

        // Open if closed, close if open
        if (openPercent < 0.01f) Open();
        else Close();

        // Trigger scene transition if applicable
        if (isTransition && sceneIndex > -1 && SceneManager.GetActiveScene().buildIndex != sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        // Trigger win state if marked as win door
        if (isWin)
        {
            GameManager.instance.youWin();
        }
    }
}

