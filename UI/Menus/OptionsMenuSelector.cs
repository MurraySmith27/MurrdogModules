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
   [SerializeField] private InputActionAsset inputActionAsset;

   [SerializeField] private string deselectActionName;

   [SerializeField] private string changeSelectionActionName;
   
   [Header("Animations")]
   [SerializeField] private CanvasGroup innerSelectorElementsCanvasGroup;

   [SerializeField] private float innersSelectorAnimationDuration = 0.1f;

   [Header("Callbacks")] 
   [SerializeField] private OptionsMenuSelectorFocusedEvent onFocusedCallbacks;
   
   [SerializeField] private OptionsMenuSelectorFocusedEvent onUnfocusedCallbacks;

   [SerializeField] private OptionMenuSelectorOptionChangedEvent onOptionChanged;

   [Header("Options")] 
   [SerializeField] private List<string> items;
   
   private InputAction _backInputAction;

   private InputAction _changeSelectionAction;
   
   private float _innerSelectorElementsCanvasGroupOriginalAlpha;

   private int _currentlySelectedItem = -1;
   
   
   #region Public Methods
   
   public void OnSelectorFocused()
   {
      TweenCanvasGroup(_innerSelectorElementsCanvasGroupOriginalAlpha, 1f, innersSelectorAnimationDuration);

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
      _currentlySelectedItem = (_currentlySelectedItem - 1) % items.Count;
      UpdateSelectedElement();
   }
   
   #endregion
   
   #region Event Methods
   
   private void Awake()
   {
      DOTween.Init();
      _backInputAction = inputActionAsset.FindAction(deselectActionName);

      _backInputAction.performed -= OnBackActionPerformed;
      _backInputAction.performed += OnBackActionPerformed;
      _backInputAction.Enable();
      
      _changeSelectionAction = inputActionAsset.FindAction(changeSelectionActionName);
      
      _changeSelectionAction.performed -= OnChangeSelectionPerformed;
      _changeSelectionAction.performed += OnChangeSelectionPerformed;
      _changeSelectionAction.Enable();

      _innerSelectorElementsCanvasGroupOriginalAlpha = innerSelectorElementsCanvasGroup.alpha;

      _currentlySelectedItem = 0;

      UpdateSelectedElement();
   }

   private void OnEnable()
   {
      if (_backInputAction != null)
      {
         _backInputAction.performed -= OnBackActionPerformed;
         _backInputAction.performed += OnBackActionPerformed;
         _backInputAction.Enable();
      }

      if (_changeSelectionAction != null)
      {
         _changeSelectionAction.performed -= OnChangeSelectionPerformed;
         _changeSelectionAction.performed += OnChangeSelectionPerformed;
         _changeSelectionAction.Enable();
      }
   }

   private void OnDisable()
   {
      if (_backInputAction != null)
      {
         _backInputAction.performed -= OnBackActionPerformed;
         _backInputAction.Disable();
      }
      
      if (_changeSelectionAction != null)
      {
         _changeSelectionAction.performed -= OnChangeSelectionPerformed;
         _changeSelectionAction.Disable();
      }
   }
   
   #endregion

   private void OnBackActionPerformed(InputAction.CallbackContext ctx)
   {
      TweenCanvasGroup(1f, _innerSelectorElementsCanvasGroupOriginalAlpha, innersSelectorAnimationDuration);
      
      foreach (Button button in buttons)
      {
         button.interactable = false;
      }
      
      onUnfocusedCallbacks?.Invoke();
   }

   private void OnChangeSelectionPerformed(InputAction.CallbackContext ctx)
   {
      Vector2 input = _changeSelectionAction.ReadValue<Vector2>();

      if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
      {
         if (input.x > 0f)
         {
            //move next
            SelectNext();
         }
         else
         {
            //move last
            SelectPrevious();
         }
      }
   }

   private void UpdateSelectedElement()
   {
      centerTextElement.text = items[_currentlySelectedItem];
      
      onOptionChanged?.Invoke(items[_currentlySelectedItem]);
   }

   private void TweenCanvasGroup(float from, float to, float duration)
   {
      innerSelectorElementsCanvasGroup.alpha = from;
      DOTween.To(
         () =>
         {
            return innerSelectorElementsCanvasGroup.alpha;
         }, 
         (float alpha) =>
         {
            innerSelectorElementsCanvasGroup.alpha = alpha;
         },
         to,
         duration
      );
   }
}
