using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable, VolumeComponentMenu("Custom Post-processing/Combination")]
public class CombinationPostProcessingVolumeComponent : VolumeComponent, IPostProcessComponent
{
    public bool IsActive() => normalOutlineStrength.value > 0;
    public bool IsTileCompatible() => true;

    [Range(0f, 1f), Tooltip("Outline thickness.")]
    public FloatParameter normalOutlineStrength = new FloatParameter(2);
    
    public ColorParameter normalOutlineColor = new ColorParameter(Color.black);
}

