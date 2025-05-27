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

    [SerializeField] private Color inShopPopupColor;
    
    [SerializeField] private Color inPopupColor;

    [SerializeField] private string backgroundHexSizePropertyName = "_HexSizeMultiplier"; 

    [SerializeField] private string backgroundColorPropertyName = "_BackgroundColor";
    [SerializeField] private string doAnimationPropertyName = "_DoAnimation";

    [Space(10)] 
    [Header("Animation")] 
    [SerializeField] private float fadeAnimationTime = 1f;
    [SerializeField] private float popupFadeAnimationTime = 0.75f;
    [SerializeField] private float resourceIncrementAnimationTime = 1f;

    [SerializeField] private Color backgroundStartingColor = Color.black;
    [SerializeField] private AnimationCurve resourceIncrementColorAnimationCurve;
    [SerializeField] private float resourceIncrementHexSizeDefault = 1f;
    [SerializeField] private float resourceIncrementHexSizeIncrease = 1.2f;
    [SerializeField] private AnimationCurve resourceIncrementHexSizeAnimationCurve;

    private bool _isInPopup = false;
    
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
        
        UIPopupSystem.Instance.OnPopupShown -= OnPopupShown;
        UIPopupSystem.Instance.OnPopupShown += OnPopupShown;
        
        UIPopupSystem.Instance.OnPopupHidden -= OnPopupHidden;
        UIPopupSystem.Instance.OnPopupHidden += OnPopupHidden;
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

        if (UIPopupSystem.IsAvailable)
        {
            UIPopupSystem.Instance.OnPopupShown -= OnPopupShown;
            UIPopupSystem.Instance.OnPopupHidden -= OnPopupHidden;
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
        Timing.RunCoroutineSingleton(FadeToColor(defaultColor, fadeAnimationTime), this.gameObject, SingletonBehavior.Overwrite);
    }

    private void FadeToBaseColor()
    {
        Timing.RunCoroutineSingleton(FadeToColor(backgroundStartingColor, fadeAnimationTime), this.gameObject, SingletonBehavior.Overwrite);
    }
    
    private void OnPopupShown(string popupName)
    {
        if (!_isInPopup)
        {
            Color color = inPopupColor;
            
            if (popupName == "ShopPopup")
                color = inShopPopupColor;
            
            Timing.RunCoroutineSingleton(FadeToColor(color, popupFadeAnimationTime), this.gameObject, SingletonBehavior.Overwrite);
            _isInPopup = true;
        }
    }

    private void OnPopupHidden(string popupName)
    {
        if (_isInPopup && UIPopupSystem.Instance.IsPopupShowing())
        {
            Timing.RunCoroutineSingleton(FadeToColor(defaultColor, popupFadeAnimationTime), this.gameObject, SingletonBehavior.Overwrite);
            _isInPopup = false;
        }
    }

    private IEnumerator<float> FadeToColor(Color color, float animationTime)
    {
        Color initialColor = backgroundMaterial.GetColor(backgroundColorPropertyName);
        
        for (float t = 0; t <= animationTime; t += Time.deltaTime * GlobalSettings.GameSpeed)
        {
            float progress = t / animationTime;
            
            backgroundMaterial.SetColor(backgroundColorPropertyName, Color.Lerp(initialColor, color, progress));

            yield return Timing.WaitForOneFrame;
        }
        
        backgroundMaterial.SetColor(backgroundColorPropertyName, color);
    }
}
