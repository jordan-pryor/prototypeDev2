using UnityEngine;

public class Door : MonoBehaviour, IInteract
{
    [SerializeField] GameObject door;
    [SerializeField, Range(0f, 1f)] float openPercent;
    [SerializeField] GameObject doorMin;
    [SerializeField] GameObject doorMax;
    [SerializeField] float openSpeed = 1f;
    [SerializeField] bool isLocked = false;

    bool isOpening = false;
    bool isClosing = false;

    void Start()
    {
        // Hide the door refs
        doorMin.SetActive(false);
        doorMax.SetActive(false);
    }

    void Update()
    {
        if (isOpening)
        {
            // Moves the open percent towards the max
            openPercent = Mathf.MoveTowards(openPercent, 1f, Time.deltaTime * openSpeed);
            if (Mathf.Approximately(openPercent, 1f))
                isOpening = false;
        }

        if (isClosing)
        {
            // Moves the open percent towards the min
            openPercent = Mathf.MoveTowards(openPercent, 0f, Time.deltaTime * openSpeed);
            if (Mathf.Approximately(openPercent, 0f))
                isClosing = false;
        }
        ApplyRotation();
    }

    void OnValidate()
    {
        ApplyRotation();
    }

    void ApplyRotation()
    {
        // Clamps
        openPercent = Mathf.Clamp01(openPercent);
        door.transform.rotation = Quaternion.Lerp(doorMin.transform.rotation, doorMax.transform.rotation, openPercent);
    }
    public void Open()
    {
        isClosing = false;
        isOpening = true;
    }
    public void Close()
    {
        isOpening = false;
        isClosing = true;
    }
    public void Interact()
    {
        if (!isOpening && !isClosing && ( !isLocked /* && key code*/) )
        {
            if (Mathf.Approximately(openPercent, 0f))
                Open();
            else
                Close();
        }
        else if(isLocked /* && key code*/)
        {
        }
    }
}

