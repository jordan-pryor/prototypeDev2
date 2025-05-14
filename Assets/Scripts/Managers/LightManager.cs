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

            if (light.type == LightType.Directional)
            {
                // Cast ray opposite the direction to check for obstruction
                if (!Physics.Raycast(position, -light.transform.forward, Mathf.Infinity, mask))
                {
                    total += light.intensity;
                }
            }
            else if (light.type == LightType.Point || light.type == LightType.Spot)
            {
                Vector3 toLight = light.transform.position - position;
                float dist = toLight.magnitude;

                // If not blocked, contribute based on inverse square falloff
                if (!Physics.Raycast(position, toLight.normalized, dist, mask))
                {
                    total += light.intensity / (dist * dist);
                }
            }
        }

        // Normalize result to 0–1
        return Mathf.Clamp01(total);
    }
}