using UnityEngine;
using System.Collections;

public class FanSpin : MonoBehaviour
{
    [SerializeField] bool isOn;
    [SerializeField] GameObject blades;
    [SerializeField] float fanSpeed = 10f;

    Coroutine spinRoutine;
    private void Start()
    {
        // Start State
        TurnFan();
    }
    public void ToggleFan()
    {
        // Toggle Fan and Call Turn Update
        isOn = !isOn;
        TurnFan();
    }
    private void TurnFan()
    {
        // Start or Stop Coroutine
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
        // Spins fan
        while (isOn)
        {
            blades.transform.Rotate(Vector3.up, fanSpeed * Time.deltaTime, Space.Self);
            yield return null;
        }
    }
}
