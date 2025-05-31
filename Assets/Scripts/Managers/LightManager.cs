using UnityEngine;

public class LightManager : MonoBehaviour
{
    public static LightManager instance;

    [Header("Occlusion")]
    [SerializeField] LayerMask mask;

    [Header("Normalization")]
    [SerializeField] float topIntensity = 100f;
    float peakIntensity;

    private Light[] lights;

    void Awake()
    {
        instance = this;
        lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        peakIntensity = Mathf.Max(1e-6f, topIntensity);
    }

    public void UpdateLights()
    {
        lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
    }

    public float GetLightLevel(Vector3 position)
    {
        float total = 0f;

        foreach (var light in lights)
        {
            if (!light.enabled || light.intensity <= 0f)
                continue;
            if (light.type == LightType.Point || light.type == LightType.Spot)
            {
                Vector3 toLight = light.transform.position - position;
                float sqrDist = toLight.sqrMagnitude;
                float range = light.range;
                if (sqrDist > range * range)
                    continue;
                Vector3 dir = toLight.normalized;
                if (light.type == LightType.Spot)
                {
                    float cosHalfAngle = Mathf.Cos(light.spotAngle * 0.5f * Mathf.Deg2Rad);
                    if (Vector3.Dot(light.transform.forward, -dir) < cosHalfAngle)
                        continue;
                }
                if (Physics.Raycast(position, dir, out var hit, Mathf.Sqrt(sqrDist), mask))
                    continue;

                total += light.intensity / sqrDist;
            }
            else
            {
                continue;
            }
        }
        if (total > peakIntensity)
            peakIntensity = total;
        return Mathf.Clamp01(total / peakIntensity);
    }
}