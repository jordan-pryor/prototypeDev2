using UnityEngine;

/// Interface for damageable objects
public interface iDamage
{
    /// Applies damage to the object
    void takeDamage(int amount);
}
