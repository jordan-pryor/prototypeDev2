using UnityEngine;

public class Vent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerController.lockJump = true;
            GameManager.instance.playerController.isCrouch = true;
            GameManager.instance.playerController.Crouch(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerController.lockJump = true;
            GameManager.instance.playerController.isCrouch = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerController.lockJump = false;
            GameManager.instance.playerController.isCrouch = false;
            GameManager.instance.playerController.Crouch(false);
        }
    }
}