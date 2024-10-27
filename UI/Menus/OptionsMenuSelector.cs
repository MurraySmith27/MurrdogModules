using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UltEvents;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsMenuSelector : MonoBehaviour
{
   [Serializable]
   public class OptionsMenuSelectorFocusedEvent : UltEvent {}

   [Serializable]
   public class OptionMenuSelectorOptionChangedEvent : UltEvent<string> {}

   [SerializeField] private TMP_Text centerTextElement;

   [SerializeField] private List<Button> buttons;
   
   [Header("Input")]
   [SerializeField] private UIInputChannel uiInputChannel;

   [SerializeField] private string deselectActionName;

   [SerializeField] private string changeSelectionActionName;
   
   [Header("Animations")]
   [SerializeField] private List<CanvasGroup> onSelectedFadeInCanvasGroups;

   [SerializeField] private float onSelectedFadeInAnimationDuration = 0.1f;

   [SerializeField] private bool useUnscaledTime = true;
   
   [Header("Callbacks")] 
   [SerializeField] private OptionsMenuSelectorFocusedEvent onFocusedCallbacks;
   
   [SerializeField] private OptionsMenuSelectorFocusedEvent onUnfocusedCallbacks;

   [SerializeField] private OptionMenuSelectorOptionChangedEvent onOptionChanged;
   
   [Header("Options")] 
   [SerializeField] private List<string> items;
   
   private InputAction _backInputAction;

   private InputAction _changeSelectionAction;
   
   private List<float> _innerSelectorElementsCanvasGroupOriginalAlphas;

   private int _currentlySelectedItem = -1;
   
   #region Public Methods
   
   public void OnSelectorFocused()
   {
      for (int i = 0; i < onSelectedFadeInCanvasGroups.Count; i++)
      {
         TweenCanvasGroup(onSelectedFadeInCanvasGroups[i], _innerSelectorElementsCanvasGroupOriginalAlphas[i], 1f, onSelectedFadeInAnimationDuration);
      }

      foreach (Button button in buttons)
      {
         button.interactable = true;
      }
      
      onFocusedCallbacks?.Invoke();
   }
   
   public void SelectPrevious()
   {
      _currentlySelectedItem = (_currentlySelectedItem + 1) % items.Count;
      UpdateSelectedElement();
   }

   public void SelectNext()
   {
      if (_currentlySelectedItem == 0)
      {
         _currentlySelectedItem = items.Count - 1;
      }
      else
      {
         _currentlySelectedItem--;
      }
      UpdateSelectedElement();
   }

   public void SetSelectedValue(string elementName)
   {
      int newIndex = items.IndexOf(elementName);

      if (newIndex == -1)
      {
         Debug.LogError($"Cannot set OptionsMenuSelector value to {elementName}! Value does not exist in items list!");
         return;
      }
      else
      {
         _currentlySelectedItem = newIndex;
         UpdateSelectedElement();
      }
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

      if (_currentlySelectedItem == -1)
      {
         _currentlySelectedItem = 0;

         UpdateSelectedElement();
      }
   }

   private void OnEnable()
   {
      uiInputChannel.BackEvent -= OnBackActionPerformed;
      uiInputChannel.BackEvent += OnBackActionPerformed;
      
      uiInputChannel.NavigateLeftEvent -= SelectPrevious;
      uiInputChannel.NavigateLeftEvent += SelectPrevious;
      
      uiInputChannel.NavigateRightEvent -= SelectNext;
      uiInputChannel.NavigateRightEvent += SelectNext;
   }

   private void OnDisable()
   {
      uiInputChannel.BackEvent -= OnBackActionPerformed;
      
      uiInputChannel.NavigateLeftEvent -= SelectPrevious;
      
      uiInputChannel.NavigateRightEvent -= SelectNext;
   }
   
   #endregion

   private void OnBackActionPerformed()
   {
      for (int i = 0; i < onSelectedFadeInCanvasGroups.Count; i++)
      {
         TweenCanvasGroup(onSelectedFadeInCanvasGroups[i], 1f, _innerSelectorElementsCanvasGroupOriginalAlphas[i], onSelectedFadeInAnimationDuration);
      }
      
      foreach (Button button in buttons)
      {
         button.interactable = false;
      }
      
      onUnfocusedCallbacks?.Invoke();
   }

   private void UpdateSelectedElement()
   {
      centerTextElement.text = items[_currentlySelectedItem];
      onOptionChanged?.Invoke(items[_currentlySelectedItem]);
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
