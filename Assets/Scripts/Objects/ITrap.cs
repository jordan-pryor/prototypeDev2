using System.Collections;
public interface ITrap
{
    // TODO: Replace int with float


    // Applies a trap effect: reduces speed for a set duration
    IEnumerator trap(float speedDecrease, float duration);
}
