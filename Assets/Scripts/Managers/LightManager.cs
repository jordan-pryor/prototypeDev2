using UnityEngine;

public class LightLevelManager : MonoBehaviour
{
    public static LightLevelManager instance;

    [SerializeField] LayerMask mask;           // Layer mask used to check for occlusion
    [SerializeField] bool auto = false;        // Auto-update lights each frame interval
    [SerializeField] int updateTime = 60;      // Interval (in frames) for updating light list

    private Light[] lights;                    // Array of all scene lights

    void Awake()
    {
        instance = this;
        lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);  // Gather all lights in scene
    }

    void Update()
    {
        // Optional auto-refresh of light list based on frame count
        if (auto && Time.frameCount % updateTime == 0)
        {
            UpdateLights();
        }
    }

    // Refresh list of lights in scene (for dynamic light changes)
    void UpdateLights()
    {
        lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
    }

    // Calculates how well-lit a given position is (0–1 range)
    public float GetLightLevel(Vector3 position)
    {
        float total = 0f;

        foreach (var light in lights)
        {
            if (!light.enabled || light.intensity <= 0f) continue;

            if (light.type == LightType.Point || light.type == LightType.Spot)
            {
                Vector3 toLight = light.transform.position - position;
                float sqrDist = toLight.sqrMagnitude;
                float range = light.range;
                if (sqrDist > range * range) continue;

                Vector3 dir = toLight / Mathf.Sqrt(sqrDist);

                if (light.type == LightType.Spot)
                {
                    float cosHalfAngle = Mathf.Cos(light.spotAngle * 0.5f * Mathf.Deg2Rad);
                    if (Vector3.Dot(light.transform.forward, -dir) < cosHalfAngle)
                        continue;
                }

                // If not blocked, contribute based on inverse square falloff
                if (!Physics.Raycast(position, dir, out RaycastHit hit, Mathf.Sqrt(sqrDist), mask))
                {
                    total += light.intensity / sqrDist;
                }
            }
        }

        // Normalize result to 0–1
        return Mathf.Clamp(total, 0, 100);
    }
}