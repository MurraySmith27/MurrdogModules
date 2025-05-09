using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class TilePreviewOverlayVisuals : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private string enterAnimatorTriggerName = "Enter";
    [SerializeField] private string exitAnimatorTriggerName = "Exit";
    [SerializeField] private string highlightEnterAnimatorTriggerName = "HighlightEnter";
    [SerializeField] private string highlightExitAnimatorTriggerName = "HighlightExit";

    [SerializeField] private float onHighlightDisappearWaitTime = 0.3f;

    [SerializeField] private float distanceDelayMultiplier = 0.1f;

    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color onSelectedColor;

    [SerializeField] private string materialColorPropertyName;

    [SerializeField] private float highlightColorAnimationTime = 0.1f;
    
    [SerializeField] private AnimationCurve highlightColorAnimCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Renderer _hexRenderer;
    private MaterialPropertyBlock _materialPropertyBlock;

    
    private bool _entered = false;
    private bool _startedExit = false;
    private bool _highlighted = false;

    private void Awake()
    {
        _hexRenderer = GetComponentInChildren<Renderer>();
        
        _materialPropertyBlock = new MaterialPropertyBlock();
        
        _hexRenderer.GetPropertyBlock(_materialPropertyBlock);
    }
    
    private float GetAngleWithCityCenter()
    {
        List<Guid> cityGuids = MapSystem.Instance.GetAllCityGuids();

        if (cityGuids.Count > 0)
        {
            Vector2Int cityCenterTilePosition = MapSystem.Instance.GetCityCenterPosition(cityGuids[0]);
            
            Vector3 worldSpace = MapUtils.GetTileWorldPositionFromGridPosition(cityCenterTilePosition);
            
            Vector3 diff = transform.position - worldSpace;

            float angle = Mathf.Atan(diff.x / diff.z);

            if (diff.x > 0)
            {
                if (diff.z < 0)
                {
                    angle = -angle; //; Mathf.PI / 2f + (Mathf.PI / 2f + angle);
                }
                else
                {
                    angle = Mathf.PI / 2f + (Mathf.PI / 2f - angle);//Mathf.PI / 2f + angle;
                }
            }
            else if (diff.z >= 0)
            {
                angle = Mathf.PI - angle;//3 * Mathf.PI / 2f + (Mathf.PI / 2f - angle);
            }
            else
            {
                angle = 3 * Mathf.PI / 2f + (Mathf.PI / 2f - angle);
            }
            
            return angle;
        }
        else return 0f;
    }
    
    public void Appear(Action onComplete = null)
    {
        if (!_entered)
        {
            //delay animation by distance from city center
            float delay = GetAngleWithCityCenter() * distanceDelayMultiplier;

            Timing.RunCoroutineSingleton(AppearCoroutine(delay, onComplete), this.gameObject,
                SingletonBehavior.Overwrite);
        }
    }

    private IEnumerator<float> AppearCoroutine(float appearDelay, Action onComplete = null)
    {
        yield return Timing.WaitForSeconds(appearDelay);
        
        this.gameObject.SetActive(true);
        AnimationUtils.ResetAnimator(animator);
        animator.SetTrigger(enterAnimatorTriggerName);
        onComplete?.Invoke();
        _entered = true;
    }

    public void Disappear(Action onComplete = null)
    {
        if (_entered)
        {
            _startedExit = true;
            float delay = GetAngleWithCityCenter() * distanceDelayMultiplier;
            Timing.RunCoroutineSingleton(DisappearCoroutine(delay, onComplete), this.gameObject,
                SingletonBehavior.Overwrite);
        }
    }
    
    private IEnumerator<float> DisappearCoroutine(float disappearDelay, Action onComplete = null)
    {
        yield return Timing.WaitForSeconds(disappearDelay);
        
        animator.SetTrigger(exitAnimatorTriggerName);
        
        yield return Timing.WaitForSeconds(onHighlightDisappearWaitTime);
        
        this.gameObject.SetActive(false);
        onComplete?.Invoke();
        _entered = false;
        _startedExit = false;
    }
    
    public void ToggleHighlight(bool isHighlighted)
    {
        if (_entered && !_startedExit && isHighlighted != _highlighted)
        {
            animator.ResetTrigger(highlightEnterAnimatorTriggerName);
            animator.ResetTrigger(highlightExitAnimatorTriggerName);
            if (isHighlighted)
            {
                FadeMaterialToColor(defaultColor, onSelectedColor);
                animator.SetTrigger(highlightEnterAnimatorTriggerName);
            }
            else
            {
                FadeMaterialToColor(onSelectedColor, defaultColor);
                animator.SetTrigger(highlightExitAnimatorTriggerName);
            }
            _highlighted = isHighlighted;
        }
    }
    
    private void FadeMaterialToColor(Color fromColor, Color toColor)
    {
        Timing.RunCoroutineSingleton(FadeMaterialToColorCoroutine(fromColor, toColor).CancelWith(this.gameObject), this.gameObject, SingletonBehavior.Overwrite);
    }

    private IEnumerator<float> FadeMaterialToColorCoroutine(Color fromColor, Color toColor)
    {
        _hexRenderer.GetPropertyBlock(_materialPropertyBlock);
        
        for (float t = 0; t < highlightColorAnimationTime; t += Time.deltaTime)
        {
            float progress = highlightColorAnimCurve.Evaluate(t / highlightColorAnimationTime);
            
            _materialPropertyBlock.SetColor(materialColorPropertyName, Color.Lerp(fromColor, toColor, progress));
            
            _hexRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return Timing.WaitForOneFrame;
        }
        
        _materialPropertyBlock.SetColor(materialColorPropertyName, toColor);
        _hexRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
