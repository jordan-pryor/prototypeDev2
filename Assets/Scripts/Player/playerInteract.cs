using UnityEngine;

public class playerInteract : MonoBehaviour
{
    [SerializeField] float range = 3f;
    [SerializeField] LayerMask mask;
    private GameObject promptInteract;

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
