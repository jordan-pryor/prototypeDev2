using UnityEngine;

public class LightLevelManager : MonoBehaviour
{
    public static LightLevelManager instance;
    [SerializeField] LayerMask mask;
    [SerializeField] bool auto = false;
    [SerializeField] int updateTime = 60;
    private Light[] lights;
    void Awake()
    {
        instance = this;
        lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
    }
    void Update()
    {
        if (auto && Time.deltaTime % updateTime == 0)
        {
            UpdateLights();
        }
    }
    void UpdateLights()
    {
        // If you use a flashlight or something
        lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
    }
    public float GetLightLevel(Vector3 position)
    {
        float total = 0f;
        foreach (var light in lights)
        {
            if (!light.enabled || light.intensity <= 0f) continue;
            if (light.type == LightType.Directional)
            {
                // Cast a ray opposite to the direction
                if (!Physics.Raycast(position, -light.transform.forward, Mathf.Infinity, mask))
                {
                    total += light.intensity;
                }
            }
            else if (light.type == LightType.Point || light.type == LightType.Spot)
            {
                Vector3 toLight = light.transform.position - position;
                float dist = toLight.magnitude;
                if (!Physics.Raycast(position, toLight.normalized, dist, mask))
                {
                    total += light.intensity / (dist * dist);
                }
            }
        }

        return Mathf.Clamp01(total);
    }
}