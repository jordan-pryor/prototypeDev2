using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private Door doorReference;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (doorReference != null && doorReference.isDoorOpen() && GameManager.instance.hasKey)
            {
                GameManager.instance.loadSceneByName(sceneToLoad);
            }
            else if(!GameManager.instance.hasKey)
            {
                //Can replace with sound or image
                Debug.Log("You Need To Find The Key!!");
            }
        }
    }
}
