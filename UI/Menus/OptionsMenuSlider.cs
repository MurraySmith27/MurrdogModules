#if USING_NEW_INPUT_SYSTEM

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UltEvents;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsMenuSlider : MonoBehaviour
{
   [Serializable]
   public class OptionsMenuSliderFocusedEvent : UltEvent {}

   [Serializable]
   public class OptionMenuSliderOptionChangedEvent : UltEvent<float> {}

   [SerializeField] private RectTransform rectTransform;

   [SerializeField] private RectTransform sliderButton;

   [SerializeField] private RectTransform sliderFill;
   
   [Header("Input")]
   [SerializeField] private UIInputChannel uiInputChannel;
   
   [Header("Animations")]
   [SerializeField] private List<CanvasGroup> onSelectedFadeInCanvasGroups;

   [SerializeField] private float onSelectedFadeInAnimationDuration = 0.1f;

   [SerializeField] private bool useUnscaledTime = true;
   
   [Header("Callbacks")] 
   [SerializeField] private OptionsMenuSliderFocusedEvent onFocusedCallbacks;
   
   [SerializeField] private OptionsMenuSliderFocusedEvent onUnfocusedCallbacks;

   [SerializeField] private OptionMenuSliderOptionChangedEvent onOptionChanged;
   
   [Header("ValueRange")] 
   [SerializeField] private Vector2 valueRange = new Vector2(0, 1);

   [SerializeField] private float incrementOnInput = 0.1f;
   
   private InputAction _backInputAction;

   private InputAction _changeSelectionAction;
   
   private List<float> _innerSelectorElementsCanvasGroupOriginalAlphas;

   private float _currentValue;

   private bool _isFocused = false;

   private bool _isDraggingWithMouse;
   
   #region Public Methods
   
   public void OnSelectorFocused()
   {
      if (!_isFocused)
      {
         _isFocused = true;
         for (int i = 0; i < onSelectedFadeInCanvasGroups.Count; i++)
         {
            TweenCanvasGroup(onSelectedFadeInCanvasGroups[i], _innerSelectorElementsCanvasGroupOriginalAlphas[i], 1f,
               onSelectedFadeInAnimationDuration);
         }

         onFocusedCallbacks?.Invoke();
      }
   }
   
   public void SelectPrevious(UIInputChannel.UIInputChannelCallbackArgs args)
   {
      if (_isFocused && !_isDraggingWithMouse)
      {
         SetSliderValue(Mathf.Clamp(_currentValue - incrementOnInput, valueRange.x, valueRange.y));
      }
   }

   public void SelectNext(UIInputChannel.UIInputChannelCallbackArgs args)
   {
      if (_isFocused && !_isDraggingWithMouse)
      {
         SetSliderValue(Mathf.Clamp(_currentValue + incrementOnInput, valueRange.x, valueRange.y));
      }
   }

   public void SetSliderValue(float sliderValue)
   {
      _currentValue = sliderValue;
      onOptionChanged?.Invoke(_currentValue);
      
      float sliderButtonParentWidth = (sliderButton.parent as RectTransform).sizeDelta.x;

      float relativeSliderValue = (sliderValue - valueRange.x) / (valueRange.y - valueRange.x);
      sliderButton.anchoredPosition = new Vector2(relativeSliderValue * sliderButtonParentWidth, sliderButton.anchoredPosition.y);

      sliderFill.anchorMax = new Vector2(relativeSliderValue, sliderFill.anchorMax.y);
   }
   
   #endregion
   
   #region Event Methods
   
   private void Start()
   {
      DOTween.Init();

      _innerSelectorElementsCanvasGroupOriginalAlphas = new List<float>();
      foreach (CanvasGroup canvasGroup in onSelectedFadeInCanvasGroups)
      {
         _innerSelectorElementsCanvasGroupOriginalAlphas.Add(canvasGroup.alpha);
      }
   }

   private void OnEnable()
   {
      uiInputChannel.BackEvent -= OnBackActionPerformed;
      uiInputChannel.BackEvent += OnBackActionPerformed;

      uiInputChannel.SelectEvent -= OnBackActionPerformed;
      uiInputChannel.SelectEvent += OnBackActionPerformed;
      
      uiInputChannel.MouseSelectEvent -= MouseSelectPerformed;
      uiInputChannel.MouseSelectEvent += MouseSelectPerformed;
      
      uiInputChannel.NavigateLeftEvent -= SelectPrevious;
      uiInputChannel.NavigateLeftEvent += SelectPrevious;
      
      uiInputChannel.NavigateRightEvent -= SelectNext;
      uiInputChannel.NavigateRightEvent += SelectNext;

      uiInputChannel.MouseUpEvent -= MouseUpPerformed;
      uiInputChannel.MouseUpEvent += MouseUpPerformed;

      uiInputChannel.MouseMoveEvent -= MouseMovePerformed;
      uiInputChannel.MouseMoveEvent += MouseMovePerformed;
   }

   private void OnDisable()
   {
      uiInputChannel.BackEvent -= OnBackActionPerformed;
      
      uiInputChannel.SelectEvent -= OnBackActionPerformed;
      
      uiInputChannel.MouseSelectEvent -= MouseSelectPerformed;
      
      uiInputChannel.NavigateLeftEvent -= SelectPrevious;
      
      uiInputChannel.NavigateRightEvent -= SelectNext;
      
      uiInputChannel.MouseUpEvent -= MouseUpPerformed;
      
      uiInputChannel.MouseMoveEvent -= MouseMovePerformed;
   }
   
   #endregion

   private void OnBackActionPerformed(UIInputChannel.UIInputChannelCallbackArgs args)
   {
      if (_isFocused)
      {
         _isFocused = false;

         _isDraggingWithMouse = false;
         
         for (int i = 0; i < onSelectedFadeInCanvasGroups.Count; i++)
         {
            TweenCanvasGroup(onSelectedFadeInCanvasGroups[i], 1f, _innerSelectorElementsCanvasGroupOriginalAlphas[i], onSelectedFadeInAnimationDuration);
         }
         
         onUnfocusedCallbacks?.Invoke();
      }
   }

   private void MouseSelectPerformed(UIInputChannel.UIInputChannelCallbackArgs args)
   {
         Vector2 mousePosition = (Vector2)args.vector2Arg;
         Vector2 localMousePosition = rectTransform.InverseTransformPoint(mousePosition);
         if (!rectTransform.rect.Contains(localMousePosition))
         {
            OnBackActionPerformed(args);
         }

         Vector2 sliderMousePosition = sliderButton.InverseTransformPoint(mousePosition);
         if (!_isDraggingWithMouse && sliderButton.rect.Contains(sliderMousePosition))
         {
            OnSelectorFocused();
            _isDraggingWithMouse = true;
         }
   }

   private void MouseUpPerformed(UIInputChannel.UIInputChannelCallbackArgs args)
   {
      if (_isDraggingWithMouse)
      {
         OnBackActionPerformed(args);
      }
   }

   private void MouseMovePerformed(UIInputChannel.UIInputChannelCallbackArgs args)
   {
      if (_isDraggingWithMouse)
      {
         Vector2 mousePosition = (Vector2)args.vector2Arg;
         Vector2 localMousePosition = sliderButton.parent.InverseTransformPoint(mousePosition);

         float sliderButtonParentWidth = (sliderButton.parent as RectTransform).rect.width;
         
         float relativeSliderValue = (localMousePosition.x / sliderButtonParentWidth) + 0.5f;
         SetSliderValue(Mathf.Clamp((relativeSliderValue * (valueRange.y - valueRange.x) + valueRange.x), valueRange.x, valueRange.y));
      }
   }

   private void TweenCanvasGroup(CanvasGroup canvasGroup, float from, float to, float duration)
   {
      if (DOTween.IsTweening(canvasGroup))
      {
         DOTween.Complete(canvasGroup);
      }
      
      canvasGroup.alpha = from;
      
      var tween = DOTween.To(
         () =>
         {
            return canvasGroup.alpha;
         },
         (float alpha) =>
         {
            canvasGroup.alpha = alpha;
         },
         to,
         duration
      );

      tween.SetTarget(canvasGroup);

      //sets to update using unscaled time;
      tween.SetUpdate(useUnscaledTime);

      tween.Play();
   }
}

#endif
