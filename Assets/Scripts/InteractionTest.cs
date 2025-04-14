using UnityEngine;

public class InteractionTest : MonoBehaviour
{
    [SerializeField] GameObject testTarget;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if(testTarget != null)
            {
                IInteract i = testTarget.GetComponent<IInteract>();
                if(i != null)
                {
                    i.Interact();
                }
            }
        }
    }
}
