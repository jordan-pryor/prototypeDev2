using UnityEngine;

public class MedKit : MonoBehaviour, IInteract
{
    [SerializeField] int healAmount = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // INTERFACE: Interact
    public void Interact()
    {
        GameManager.instance.playerController.HP += healAmount;
        GameManager.instance.playerController.updatePlayerUI();
        Destroy(gameObject);
    }
}
