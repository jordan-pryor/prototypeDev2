using UnityEngine;

public class playerInteract : MonoBehaviour
{
    [SerializeField] float range = 3f;          // How far the player can interact
    [SerializeField] LayerMask mask;            // Which layers to consider for interaction

    private GameObject promptInteract;          // UI prompt (from GameManager)

    IInteract target;                           // Current interactable in sight
    IInteract grabTarget;                       // Target being interacted with after animation

    void Update()
    {
        CheckInteractable();

        // If interactable is in range and E is pressed
        if (target != null && Input.GetKeyDown(KeyCode.E))
        {
            // If it's a pickup, unequip current item
            if ((target as Component).CompareTag("Pickup"))
                GameManager.instance.playerController.inv.Unequip();

            // Trigger grab animation
            GameManager.instance.playerController.anim.SetTrigger("isGrabbing");
            grabTarget = target;
        }
    }

    // Called from animation event after grab animation completes
    public void Grabbed()
    {
        if (grabTarget == null) return;

        grabTarget.Interact();   // Call interaction
        grabTarget = null;
    }

    // Casts a ray to detect interactables in front of the player
    void CheckInteractable()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, mask))
        {
            IInteract interactable = hit.collider.GetComponent<IInteract>();
            if (interactable != null)
            {
                if (target != interactable)
                {
                    target = interactable;
                    GameManager.instance.promptInteract.SetActive(true); // Show UI prompt
                }
                return;
            }
        }

        // No valid target found
        target = null;
        GameManager.instance.promptInteract.SetActive(false); // Hide UI prompt
    }
}
