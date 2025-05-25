using System.Collections;
using System.Collections.Generic;
using MEC;
using Unity.VisualScripting;
using UnityEngine;

public class BackgroundAnimator : MonoBehaviour
{
    [SerializeField] private ResourceVisualDataSO resourceVisualData;

    [SerializeField] private Material backgroundMaterial;
    
    [SerializeField] private Color defaultColor;

    [SerializeField] private string backgroundHexSizePropertyName = "_HexSizeMultiplier"; 

    [SerializeField] private string backgroundColorPropertyName = "_BackgroundColor";
    [SerializeField] private string doAnimationPropertyName = "_DoAnimation";

    [Space(10)] 
    [Header("Animation")] 
    [SerializeField] private float fadeAnimationTime = 1f;
    [SerializeField] private float resourceIncrementAnimationTime = 1f;

    [SerializeField] private Color backgroundStartingColor = Color.black;
    [SerializeField] private AnimationCurve resourceIncrementColorAnimationCurve;
    [SerializeField] private float resourceIncrementHexSizeDefault = 1f;
    [SerializeField] private float resourceIncrementHexSizeIncrease = 1.2f;
    [SerializeField] private AnimationCurve resourceIncrementHexSizeAnimationCurve;

    private void Awake()
    {
        backgroundMaterial.SetColor(backgroundColorPropertyName, defaultColor);
        backgroundMaterial.SetFloat(backgroundHexSizePropertyName, resourceIncrementHexSizeDefault);
    }
    
    private void Start()
    {
        BloomingHarvestController.Instance.OnHarvestStart -= FadeToBaseColor;
        BloomingHarvestController.Instance.OnHarvestStart += FadeToBaseColor;
        
        BloomingHarvestController.Instance.OnHarvestEnd -= ResetToDefaultColor;
        BloomingHarvestController.Instance.OnHarvestEnd += ResetToDefaultColor;
        
        BloomingHarvestResourceDisplay.Instance.OnResourceIncrementAnimationTriggered -= OnResourceIncrementAnimationTriggered;
        BloomingHarvestResourceDisplay.Instance.OnResourceIncrementAnimationTriggered += OnResourceIncrementAnimationTriggered;
    }

    private void OnDestroy()
    {
        if (BloomingHarvestResourceDisplay.IsAvailable)
        {
            BloomingHarvestResourceDisplay.Instance.OnResourceIncrementAnimationTriggered -=
                ChangeBackgroundColorToResourceColor;
        }

        if (BloomingHarvestController.IsAvailable)
        {
            BloomingHarvestController.Instance.OnHarvestStart -= FadeToBaseColor;
            BloomingHarvestController.Instance.OnHarvestEnd -= ResetToDefaultColor;
        }
    }

    private void OnResourceIncrementAnimationTriggered(ResourceType resourceType)
    {
        ChangeBackgroundColorToResourceColor(resourceType);
    }

    private IEnumerator<float> BackgroundOnResourceIncrementedAnimation(Color color)
    {
        float initialHexSize = resourceIncrementHexSizeDefault;
        
        backgroundMaterial.SetInt(doAnimationPropertyName, 0);
        
        for (float t = 0; t <= resourceIncrementAnimationTime; t += Time.deltaTime * GlobalSettings.GameSpeed)
        {
            float progress = t / resourceIncrementAnimationTime;
            
            backgroundMaterial.SetColor(backgroundColorPropertyName, Color.Lerp(backgroundStartingColor, color, resourceIncrementColorAnimationCurve.Evaluate(progress)));
            backgroundMaterial.SetFloat(backgroundHexSizePropertyName, initialHexSize + Mathf.Lerp(0, resourceIncrementHexSizeIncrease, resourceIncrementHexSizeAnimationCurve.Evaluate(progress)));

            yield return Timing.WaitForOneFrame;
        }
        
        backgroundMaterial.SetInt(doAnimationPropertyName, 1);
        backgroundMaterial.SetColor(backgroundColorPropertyName, Color.Lerp(backgroundStartingColor, color, resourceIncrementColorAnimationCurve.Evaluate(1f)));
        backgroundMaterial.SetFloat(backgroundHexSizePropertyName, Mathf.Lerp(initialHexSize, resourceIncrementHexSizeIncrease, resourceIncrementHexSizeAnimationCurve.Evaluate(1f)));
    }
    
    private void ChangeBackgroundColorToResourceColor(ResourceType resourceType)
    {
        Color resourceColor = resourceVisualData.GetColorForResourceItem(resourceType);
        
        Timing.RunCoroutineSingleton(BackgroundOnResourceIncrementedAnimation(resourceColor), this.gameObject, SingletonBehavior.Overwrite);
    }

    private void ResetToDefaultColor()
    {
        Timing.RunCoroutineSingleton(FadeToColor(defaultColor), this.gameObject, SingletonBehavior.Overwrite);
    }

    private void FadeToBaseColor()
    {
        Timing.RunCoroutineSingleton(FadeToColor(backgroundStartingColor), this.gameObject, SingletonBehavior.Overwrite);
    }

    private IEnumerator<float> FadeToColor(Color color)
    {
        Color initialColor = backgroundMaterial.GetColor(backgroundColorPropertyName);
        
        for (float t = 0; t <= fadeAnimationTime; t += Time.deltaTime * GlobalSettings.GameSpeed)
        {
            float progress = t / fadeAnimationTime;
            
            backgroundMaterial.SetColor(backgroundColorPropertyName, Color.Lerp(initialColor, color, progress));

            yield return Timing.WaitForOneFrame;
        }
        
        backgroundMaterial.SetColor(backgroundColorPropertyName, color);
    }
}
