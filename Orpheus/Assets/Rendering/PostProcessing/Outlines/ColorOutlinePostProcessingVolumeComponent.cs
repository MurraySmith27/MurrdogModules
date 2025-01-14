using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Custom Post-processing/ColorOutline")]
public class ColorOutlinePostProcessingVolumeComponent : VolumeComponent, IPostProcessComponent
{
    public bool IsActive() => normalStrength.value > 0f || colorStrength.value > 0f;
    public bool IsTileCompatible() => true;

    [Range(0.01f, 10f), Tooltip("The threshold for normal-difference outlines.")]
    public FloatParameter normalThreshold = new FloatParameter(0.1f);

    [Range(0f, 1f), Tooltip("The strength of normal-difference outlines.")]
    public FloatParameter normalStrength = new FloatParameter(0f);
    
    [Range(0.01f, 10f), Tooltip("The threshold for color-difference outlines.")]
    public FloatParameter colorThreshold = new FloatParameter(0.1f);

    [Range(0f, 1f), Tooltip("The strength of color-difference outlines.")]
    public FloatParameter colorStrength = new FloatParameter(0f);
    
    [Tooltip("Outline color.")] 
    public ColorParameter color = new ColorParameter(Color.black);
}
