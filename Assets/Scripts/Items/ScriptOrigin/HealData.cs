using UnityEngine;

[CreateAssetMenu(menuName = "Items/Heal")]
public class HealData : BaseData
{
    [Range(0, 50)] public int instantAmt;
    [Range(0, 10)] public int hotAmt;
    [Range(1, 10)] public int sec;
    public Sound healSound;
}

