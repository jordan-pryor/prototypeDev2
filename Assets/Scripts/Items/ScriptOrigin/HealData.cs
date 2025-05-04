using UnityEngine;

[CreateAssetMenu(menuName = "Items/Heal")]
public class HealData : BaseData
{
    public float instantAmt;     // Instant healing amount
    public float hotAmt;         // Heal-over-time amount per second
    public float sec;            // Duration of heal-over-time effect
}

