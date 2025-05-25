using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RoomColor : MonoBehaviour
{
    public Volume volume;
    public Color shadowColor = Color.cyan;
    public Color midtoneColor = Color.green;
    public Color highlightColor = Color.yellow;
    [Range(-1f, 1f)] public float shadowIntensity = 0f;
    [Range(-1f, 1f)] public float midtoneIntensity = 0f;
    [Range(-1f, 1f)] public float highlightIntensity = 0f;

    private VolumeProfile runtimeProfile;
    private ShadowsMidtonesHighlights smh;

    private void Start()
    {
        if (volume == null || volume.sharedProfile == null)
        {
            Debug.LogError("Volume or sharedProfile not assigned.");
            return;
        }

        // Clone the profile to avoid editing the shared asset
        runtimeProfile = Instantiate(volume.sharedProfile);
        volume.profile = runtimeProfile;

        // Get or add the component
        if (!runtimeProfile.TryGet(out smh))
        {
            smh = runtimeProfile.Add<ShadowsMidtonesHighlights>();
        }

        smh.active = true;
        smh.shadows.overrideState = true;
        smh.midtones.overrideState = true;
        smh.highlights.overrideState = true;

        ApplyColors(); // Now apply the runtime-safe values
    }

    public void ApplyColors()
    {
        if (smh == null) return;

        smh.shadows.value = new Vector4(shadowColor.r, shadowColor.g, shadowColor.b, shadowIntensity);
        smh.midtones.value = new Vector4(midtoneColor.r, midtoneColor.g, midtoneColor.b, midtoneIntensity);
        smh.highlights.value = new Vector4(highlightColor.r, highlightColor.g, highlightColor.b, highlightIntensity);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) ApplyColors();
    }
#endif
}