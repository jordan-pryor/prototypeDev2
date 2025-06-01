using System.Collections;
public interface ITrap
{
    // Applies a trap effect: reduces speed for a set duration
    void trapTrigger(float speedDecrease, float duration);
}
