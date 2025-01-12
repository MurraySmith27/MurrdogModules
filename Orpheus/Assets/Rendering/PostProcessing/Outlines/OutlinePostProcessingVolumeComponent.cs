using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Custom Post-processing/Outline")]
public class OutlinePostProcessingVolumeComponent : VolumeComponent, IPostProcessComponent
{
    public bool IsActive() => thickness.value > 0;
    public bool IsTileCompatible() => true;

    [Range(0f, 5f), Tooltip("Outline thickness in pixels.")]
    public IntParameter thickness = new IntParameter(2);
    
    [Range(0f, 5f), Tooltip("Outline edge start.")]
    public FloatParameter edge = new FloatParameter(0.1f);
    
    [Range(0f, 1f), Tooltip("Outline smoothness transition on close objects.")]
    public FloatParameter transitionSmoothness = new FloatParameter(0.1f);

    [Tooltip("Outline color.")] 
    public ColorParameter color = new ColorParameter(Color.black);
}
