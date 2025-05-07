using UnityEngine;
using System.Collections;

public class FanSpin : MonoBehaviour
{
    [SerializeField] bool isOn;                  // Current power state
    [SerializeField] GameObject blades;          // Fan blade object to rotate
    [SerializeField] float fanSpeed = 10f;       // Rotation speed of blades

    Coroutine spinRoutine;

    private void Start()
    {
        // Initialize based on isOn state
        TurnFan();
    }

    public void ToggleFan()
    {
        // Toggle power state and update behavior
        isOn = !isOn;
        TurnFan();
    }

    private void TurnFan()
    {
        // Start or stop rotation coroutine based on isOn
        if (isOn)
        {
            spinRoutine = StartCoroutine(SpinFan());
        }
        else
        {
            if (spinRoutine != null) StopCoroutine(spinRoutine);
        }
    }

    IEnumerator SpinFan()
    {
        // Continuously rotate blades while fan is on
        while (isOn)
        {
            blades.transform.Rotate(Vector3.up, fanSpeed * Time.deltaTime, Space.Self);
            yield return null;
        }
    }
}
