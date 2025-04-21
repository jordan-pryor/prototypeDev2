using UnityEngine;

public class playerInteract : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] float range = 3f;
    [SerializeField] LayerMask mask;
    [SerializeField] GameObject promptInteract;

    IInteract target;

    // Update is called once per frame
    void Update()
    {
        CheckInteractable();
        if(target != null && Input.GetKeyDown(KeyCode.E))
        {
            target.Interact();
        }
    }
    void CheckInteractable()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, mask))
        {
            IInteract interactable = hit.collider.GetComponent<IInteract>();
            if(interactable != null)
            {
                if (target != interactable)
                {
                    target = interactable;
                    promptInteract.SetActive(true);
                }
                return;
            }
        }
        target = null;
        promptInteract.SetActive(false);
    }
}
