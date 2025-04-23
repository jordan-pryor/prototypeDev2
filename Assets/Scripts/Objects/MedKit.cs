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
        //GameManager.instance.PlayerController.HP += healAmount;
        //GameManager.instance.PlayerController.updatePlayerUI();
        Destroy(gameObject);
    }
}
