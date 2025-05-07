using UnityEngine;

public interface IUse
{
    // Called when the object is used
    // 'primary' is true for main action, false for secondary
    void Use(bool primary);
}
