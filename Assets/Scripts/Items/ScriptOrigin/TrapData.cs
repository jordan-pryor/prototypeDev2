using UnityEngine;


[CreateAssetMenu(menuName = "Items/Traps")]
public class TrapData : BaseData
{
    public GameObject trapToSet;
    public float slowMultiplier;
    public int trapDuration;
}
