using UnityEngine;

public class playerInteract : MonoBehaviour
{
    [SerializeField] float range = 3f;
    [SerializeField] LayerMask mask;
    private GameObject promptInteract;

    IInteract target;
    IInteract grabTarget;
    // Update is called once per frame
    void Update()
    {
        CheckInteractable();
        if(target != null && Input.GetKeyDown(KeyCode.E))
        {
            if((target as Component).CompareTag("Pickup")) GameManager.instance.playerController.inv.Unequip();
            GameManager.instance.playerController.anim.SetTrigger("isGrabbing");
            grabTarget = target;
        }
    }
    public void Grabbed()
    {
        if (grabTarget == null) return;
        grabTarget.Interact();
        grabTarget = null;
    }
    void CheckInteractable()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, mask))
        {
            IInteract interactable = hit.collider.GetComponent<IInteract>();
            if(interactable != null)
            {
                if (target != interactable)
                {
                    target = interactable;
                    GameManager.instance.promptInteract.SetActive(true);
                }
                return;
            }
        }
        target = null;
        GameManager.instance.promptInteract.SetActive(false);
    }
}
