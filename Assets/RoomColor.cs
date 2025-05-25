using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RoomColor : MonoBehaviour
{
    public Volume volume;
    public int roomIndex;
    public Color shadowColor = Color.cyan;
    public Color midtoneColor = Color.green;
    public Color highlightColor = Color.yellow;
    [Range(-1f, 1f)] public float shadowIntensity = 0f;
    [Range(-1f, 1f)] public float midtoneIntensity = 0f;
    [Range(-1f, 1f)] public float highlightIntensity = 0f;
    private VolumeProfile runtimeProfile;
    private ShadowsMidtonesHighlights smh;
    private static Dictionary<int, Vector4[]> savedColors = new();
    private void Start()
    {
        if (volume == null || volume.sharedProfile == null)
        {
            return;
        }
        runtimeProfile = Instantiate(volume.sharedProfile);
        volume.profile = runtimeProfile;
        if (!runtimeProfile.TryGet(out smh)) smh = runtimeProfile.Add<ShadowsMidtonesHighlights>();
        smh.active = true;
        smh.shadows.overrideState = true;
        smh.midtones.overrideState = true;
        smh.highlights.overrideState = true;
        Load();
        ApplyColors();
    }
    public void ApplyColors()
    {
        if (smh == null) return;

        smh.shadows.value = new Vector4(shadowColor.r, shadowColor.g, shadowColor.b, shadowIntensity);
        smh.midtones.value = new Vector4(midtoneColor.r, midtoneColor.g, midtoneColor.b, midtoneIntensity);
        smh.highlights.value = new Vector4(highlightColor.r, highlightColor.g, highlightColor.b, highlightIntensity);
    }
    public void Save()
    {
        savedColors[roomIndex] = new Vector4[]
        {
            new(shadowColor.r, shadowColor.g, shadowColor.b, shadowIntensity),
            new(midtoneColor.r, midtoneColor.g, midtoneColor.b, midtoneIntensity),
            new(highlightColor.r, highlightColor.g, highlightColor.b, highlightIntensity)
        };
    }
    public void Load()
    {
        if (!savedColors.TryGetValue(roomIndex, out Vector4[] values)) return;
        smh.shadows.value = values[0];
        smh.midtones.value = values[1];
        smh.highlights.value = values[2];
        shadowColor = new Color(values[0].x, values[0].y, values[0].z);
        midtoneColor = new Color(values[1].x, values[1].y, values[1].z);
        highlightColor = new Color(values[2].x, values[2].y, values[2].z);
        shadowIntensity = values[0].w;
        midtoneIntensity = values[1].w;
        highlightIntensity = values[2].w;
    }
    [ContextMenu("Save Room Colors")]
    private void ContextSave() => Save();

    [ContextMenu("Load Room Colors")]
    private void ContextLoad() => Load();

    [ContextMenu("Apply Room Colors")]
    private void ContextApply() => ApplyColors();
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) ApplyColors();
    }
#endif
}
