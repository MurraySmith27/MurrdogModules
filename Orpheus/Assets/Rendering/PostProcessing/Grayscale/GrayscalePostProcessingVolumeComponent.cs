using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable, VolumeComponentMenu("Custom Post-processing/Grayscale")]
public class GrayscalePostProcessingVolumeComponent : VolumeComponent, IPostProcessComponent
{
    public bool IsActive() => effectStrength.value > 0f;
    public bool IsTileCompatible() => true;

    [Range(0f, 1f), Tooltip("The strength of the effect.")]
    public FloatParameter effectStrength = new FloatParameter(1f);
    
    [Range(0f, 1f), Tooltip("The lightness of the effect.")]
    public FloatParameter lightness = new FloatParameter(1f);
    
    [Tooltip("The coefficients of the grayscale effect.")]
    public Vector3Parameter grayScaleCoefficients = new Vector3Parameter(new Vector3(0.2126f, 0.7152f, 0.0722f));
    
}
