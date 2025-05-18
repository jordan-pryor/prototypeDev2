using UnityEngine;


[CreateAssetMenu(menuName = "Items/Traps")]
public class TrapData : BaseData
{
    public GameObject trapToSet;

    [Header("Effects")]
    public int damageAmount = 0;
    public float stunDuration = 0f;
    public float speedDecrease = 0f;

    [Header("Reset/Pickup")]
    public float resetDelay = 5f;   //Time before rearm's
    public bool persistent = false; //Stays on ground or not
    public int maxUses = 1;         //How many times it can trigger
}
