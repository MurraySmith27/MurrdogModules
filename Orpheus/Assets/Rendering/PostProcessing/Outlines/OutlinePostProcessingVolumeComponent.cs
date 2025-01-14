using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Custom Post-processing/Outline")]
public class OutlinePostProcessingVolumeComponent : VolumeComponent, IPostProcessComponent
{
    public bool IsActive() => thickness.value > 0;
    public bool IsTileCompatible() => true;

    [Range(0f, 0.5f), Tooltip("Outline thickness.")]
    public FloatParameter thickness = new FloatParameter(2);
    
    [Range(0f, 1f), Tooltip("Depth threshold, higher means more frequent outlines.")]
    public FloatParameter depthThreshold = new FloatParameter(0.1f);
    
    [Range(0f, 500f), Tooltip("Overall depth strength, higher means darker outlines.")]
    public FloatParameter depthStrength = new FloatParameter(0.1f);
    
    [Range(0f, 1f), Tooltip("Overall depth thickness, higher means thicker outlines at shallower depths.")]
    public FloatParameter depthThickness = new FloatParameter(0.1f);

    [Tooltip("Outline color.")] 
    public ColorParameter color = new ColorParameter(Color.black);
}
