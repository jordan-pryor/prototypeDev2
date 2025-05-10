using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    

    // Trap behavior types
    private enum trapType { noreset, auto, manual }

    // Trap state stages
    public enum trapState { inactive, reset, active }

    [Header("Trap Options")]
    [SerializeField] Renderer model;             // Visual indicator
    [SerializeField] trapType type;              // Reset behavior type
    [SerializeField] float speedMult;            // Movement slowdown multiplier
    [SerializeField] int trapDurationSeconds = 5;// Time player is trapped
    [SerializeField] int trapResetSeconds = 2;   // Time before trap reactivates

    public trapState curState = trapState.inactive; // Initial state

    private bool inTrigger;                      // Tracks if player is inside trigger
    WaitForSeconds resetWait;                    // Cached wait for reset duration

    private void Start()
    {
        //resetWait = new WaitForSeconds(trapResetSeconds);
        OnStateChange();                         // Update trap visuals
    }

    void Update()
    {
        // If player is inside trigger and trap is inactive
        if (inTrigger && curState == trapState.inactive)
        {
            GameManager.instance.promptInteract.SetActive(true);

            // Player manually resets trap
            if (Input.GetButtonDown("Interact"))
            {
                StartCoroutine(resetTrap());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        //inTrigger = true;

        // If trap is active and enemy is trapable
        ITrap itrap = other.GetComponent<ITrap>();
        if (itrap != null && curState == trapState.active)
        {
            StartCoroutine(itrap.trap(speedMult, trapDurationSeconds));
            curState = trapState.inactive;
            OnStateChange();

            if (type == trapType.auto)
            {
                StartCoroutine(resetTrap());
            }
        }

        // Handle trap deactivation and optional auto-reset. Leave incase want a pickup after setting it. 
        //if (type != trapType.noreset)
        //{
        //curState = trapState.inactive;
        //OnStateChange();

        //if (type == trapType.auto)
        //{
        //StartCoroutine(resetTrap());
        //}
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        inTrigger = false;
        GameManager.instance.promptInteract.SetActive(false);
    }

    // Handles resetting the trap over time
    IEnumerator resetTrap()
    {
        curState = trapState.reset;
        OnStateChange();
        yield return resetWait;
        curState = trapState.active;
        OnStateChange();
    }

    // Updates trap model color based on current state
    public void OnStateChange()
    {
        if (model != null)
        {
            switch (curState)
            {
                case trapState.active:
                    model.material.color = Color.red;
                    break;
                case trapState.inactive:
                    model.material.color = Color.black;
                    break;
                case trapState.reset:
                    model.material.color = Color.gray;
                    break;
            }
        }
    }
}