using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class RoomColor : MonoBehaviour
{
    public Volume volume;
    public Color shadowColor = Color.cyan;
    public Color midtoneColor = Color.green;
    public Color highlightColor = Color.yellow;
    [Range(-1f, 1f)] public float shadowIntensity = 0f;
    [Range(-1f, 1f)] public float midtoneIntensity = 0f;
    [Range(-1f, 1f)] public float highlightIntensity = 0f;
    private void OnValidate()
    {
        ApplyColors();
    }
    public void ApplyColors()
    {
        if (volume == null) return;
        if (volume.profile.TryGet<ShadowsMidtonesHighlights>(out var vol))
        {
            vol.active = true;
            vol.shadows.overrideState = true;
            vol.midtones.overrideState = true;
            vol.highlights.overrideState = true;
            vol.shadows.value = new Vector4(
                shadowColor.r, shadowColor.g, shadowColor.b, shadowIntensity);
            vol.midtones.value = new Vector4(
                midtoneColor.r, midtoneColor.g, midtoneColor.b, midtoneIntensity);
            vol.highlights.value = new Vector4(
                highlightColor.r, highlightColor.g, highlightColor.b, highlightIntensity);
        }
    }
}