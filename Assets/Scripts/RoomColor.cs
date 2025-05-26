using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class RoomColor : MonoBehaviour
{
    public VolumeProfile profile;

    public Color shadows = Color.white;
    [Range(-1f, 1f)] public float shadowIntensity = 0f;

    public Color midtones = Color.white;
    [Range(-1f, 1f)] public float midtoneIntensity = 0f;

    public Color highlights = Color.white;
    [Range(-1f, 1f)] public float highlightIntensity = 0f;

    void OnValidate()
    {
        if (profile == null) return;

        if (profile.TryGet(out ShadowsMidtonesHighlights smh))
        {
            smh.active = true;

            smh.shadows.overrideState = true;
            smh.midtones.overrideState = true;
            smh.highlights.overrideState = true;

            smh.shadows.value = new Vector4(shadows.r, shadows.g, shadows.b, shadowIntensity);
            smh.midtones.value = new Vector4(midtones.r, midtones.g, midtones.b, midtoneIntensity);
            smh.highlights.value = new Vector4(highlights.r, highlights.g, highlights.b, highlightIntensity);
        }
    }
}