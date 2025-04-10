using System.Collections;
using System.Collections.Generic;
using MEC;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UISubmenu : MonoBehaviour
{
    [SerializeField] private OrpheusUIInputChannel inputChannel;
    
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string submenuEnterTriggerName = "Enter";
    [SerializeField] private string submenuExitTriggerName = "Exit";
    [SerializeField] private RectTransform parentRectTransform;
    [SerializeField] private float parentRectTransformScaleIncrease = 1.2f;
    [SerializeField] private float parentRectTransformScaleIncreaseTime = 0.15f;
    [SerializeField] private AnimationCurve parentRectTransformScaleIncreaseAnimCurve = AnimationCurve.EaseInOut(0f,0f,1f,1f);

    private bool _isShowing = false;
    
    private void Start()
    {
        inputChannel.LeftMouseUpEvent -= OnLeftMouseClick;
        inputChannel.LeftMouseUpEvent += OnLeftMouseClick;
    }

    private void OnDestroy()
    {
        if (inputChannel != null)
        {
            inputChannel.LeftMouseUpEvent -= OnLeftMouseClick;
        }
    }
    private void OnLeftMouseClick(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        if (RectTransformUtils.IsMouseOverRectTransform(parentRectTransform))
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    
    public void Show()
    {
        if (!_isShowing)
        {
            AnimationUtils.ResetAnimator(animator);
            animator.SetTrigger(submenuEnterTriggerName);

            Timing.RunCoroutineSingleton(AnimateParentRectTransformScale(1f, parentRectTransformScaleIncrease).CancelWith(parentRectTransform.gameObject),
                this.gameObject, SingletonBehavior.Overwrite);
            _isShowing = true;
        }
    }

    private IEnumerator<float> AnimateParentRectTransformScale(float initialScale, float destinationScale)
    {
        Vector3 startingScale = new Vector3(initialScale, initialScale, initialScale);
        Vector3 targetScale = new Vector3(destinationScale, destinationScale, destinationScale);

        for (float t = 0f; t <= parentRectTransformScaleIncreaseTime; t += Time.deltaTime)
        {
            float progress = parentRectTransformScaleIncreaseAnimCurve.Evaluate(t / parentRectTransformScaleIncreaseTime);

            if (parentRectTransform == null)
            {
                yield break;
            }
            
            parentRectTransform.localScale = Vector3.Lerp(startingScale, targetScale, progress);

            yield return Timing.WaitForOneFrame;
        }

        parentRectTransform.localScale = targetScale;
    }

    public void Hide()
    {
        if (_isShowing)
        {
            animator.SetTrigger(submenuExitTriggerName);

            Timing.RunCoroutineSingleton(AnimateParentRectTransformScale(parentRectTransformScaleIncrease, 1f),
                this.gameObject, SingletonBehavior.Overwrite);

            _isShowing = false;
        }
    }
    
    
}
