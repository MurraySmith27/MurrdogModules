#if USING_NEW_INPUT_SYSTEM
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum PanelShowBehaviour
{
   KEEP_PREVIOUS,
   HIDE_PREVIOUS,
   SHOW_NEXT,
   FORCE_CLEAR_QUEUE,
}

public class UIPopupSystem : Singleton<UIPopupSystem>
{
   [SerializeField] private UIInputChannel inputChannel;
   
   public List<UIPopup> uiPopups;

   private LinkedList<(string, UIPopupComponent)> _popupQueue = new();

   private (string, UIPopupComponent) _activePopup;

   private Dictionary<string, UIPopupComponent> _popupInstancePool = new();

   private List<(UIInputType, UnityAction<UIInputChannel.UIInputChannelCallbackArgs>)> _showPopupActions = new();
   
   private List<(UIInputType, UnityAction<UIInputChannel.UIInputChannelCallbackArgs>)> _hidePopupActions = new();

   private bool _isCurrentlyStashed = false;

   public event Action<string> OnPopupShown;
   public event Action<string> OnPopupHidden;

   void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
   {
      SetUpShowAndHideCallbacks();
   }

   void OnSceneUnloaded(Scene scene)
   {
      ResetInputActions();
   }

   public override void OnAwake()
   {
      SetUpShowAndHideCallbacks();

      SceneManager.sceneLoaded -= OnSceneLoaded;
      SceneManager.sceneLoaded += OnSceneLoaded;
      
      SceneManager.sceneUnloaded -= OnSceneUnloaded;
      SceneManager.sceneUnloaded += OnSceneUnloaded;
   }
   
   private void SetUpShowAndHideCallbacks()
   {
      ResetInputActions();
      
      string currentSceneName = SceneManager.GetActiveScene().name;
      
      if (inputChannel != null)
      {
         foreach (UIPopup uiPopup in uiPopups)
         {
            if (!uiPopup.excludedSceneNames.Contains(currentSceneName))
            {
               string popupId = uiPopup.id; 

               UnityAction<UIInputChannel.UIInputChannelCallbackArgs> showCallback = (UIInputChannel.UIInputChannelCallbackArgs args) =>
               {
                  ShowPopup(popupId);
               };
               
               foreach (UIInputType showInput in uiPopup.showPopupInputs)
               {
                  _showPopupActions.Add((showInput, showCallback));
                  
                  RegisterCallbackToInput(showCallback, showInput);
               }

               UnityAction<UIInputChannel.UIInputChannelCallbackArgs> hideCallback = (UIInputChannel.UIInputChannelCallbackArgs args) =>
               {
                  HidePopup(popupId);
               };
               
               foreach (UIInputType hideInput in uiPopup.hidePopupInputs)
               {
                  _hidePopupActions.Add((hideInput, hideCallback));
                  
                  RegisterCallbackToInput(hideCallback, hideInput);
               }
            }
         }
      }
      else
      {
         Debug.LogError("UIPopupSystem has not been assigned a UIInputChannel! Popup show and hide actions might not work.");
      }
   }

   private void ResetInputActions()
   {
      foreach ((UIInputType, UnityAction<UIInputChannel.UIInputChannelCallbackArgs>) showPopupAction in _showPopupActions)
      {
         DeregisterCallbackFromInput(showPopupAction.Item2, showPopupAction.Item1);
      }
      
      foreach ((UIInputType, UnityAction<UIInputChannel.UIInputChannelCallbackArgs>) hidePopupAction in _hidePopupActions)
      {
         DeregisterCallbackFromInput(hidePopupAction.Item2, hidePopupAction.Item1);
      }
      
      _showPopupActions.Clear();
      _hidePopupActions.Clear();
   }

   public UIPopupComponent ShowPopup(string popupId, PanelShowBehaviour showBehaviour = PanelShowBehaviour.KEEP_PREVIOUS)
   {
      if (_isCurrentlyStashed)
      {
         Debug.LogError($"Cannot show popup of type {popupId}. System is currently stashed");
         return null;
      }

      UIPopup uiPopup = uiPopups.FirstOrDefault(popup => popup.id == popupId);

      if (uiPopup != null)
      {
         //see if we've already queued this popup.
         if (_popupQueue.FirstOrDefault(popup => popup.Item1 == popupId).Item2 != null)
         {
            return null;
         }

         if (!_popupInstancePool.ContainsKey(popupId))
         {
            if (uiPopup.prefab != null)
            {
               uiPopup.prefab.SetActive(false);
               _popupInstancePool.Add(popupId, Instantiate(uiPopup.prefab, transform).GetComponent<UIPopupComponent>());
            } 
            else if (uiPopup.gameObjectInstance != null)
            {
               _popupInstancePool.Add(popupId, uiPopup.gameObjectInstance.GetComponent<UIPopupComponent>());
            }
         }
         
         UIPopupComponent newPopup = _popupInstancePool[popupId];
         
         if (_popupQueue.Count == 0 && _activePopup.Item2 == null)
         {
            _activePopup = (popupId, newPopup.GetComponent<UIPopupComponent>());
            
            newPopup.OnPopupShow();
            OnPopupShown?.Invoke(popupId);
         }
         else if (_activePopup.Item1 != popupId)
         {
            if (showBehaviour == PanelShowBehaviour.FORCE_CLEAR_QUEUE)
            {
               if (_activePopup.Item2 != null)
               {
                  _activePopup.Item2.OnPopupHide();
                  OnPopupHidden?.Invoke(_activePopup.Item1);

                  _popupQueue.Clear();
               }

               _activePopup = (popupId, newPopup.GetComponent<UIPopupComponent>());
               newPopup.OnPopupShow();
               OnPopupShown?.Invoke(popupId);
            }
            else if (showBehaviour == PanelShowBehaviour.HIDE_PREVIOUS)
            {
               if (_activePopup.Item2 != null)
               {
                  _activePopup.Item2.OnPopupHide();
                  OnPopupHidden?.Invoke(_activePopup.Item1);

                  _popupQueue.AddFirst(_activePopup);

                  _activePopup = ("", null);
               }
         
               _activePopup = (popupId, newPopup.GetComponent<UIPopupComponent>());
         
               newPopup.OnPopupShow();
               OnPopupShown?.Invoke(popupId);
            }
            else
            {
               if (showBehaviour == PanelShowBehaviour.SHOW_NEXT)
               {
                  _popupQueue.AddFirst((popupId, newPopup.GetComponent<UIPopupComponent>()));
               }
               else
               {
                  _popupQueue.AddLast((popupId, newPopup.GetComponent<UIPopupComponent>()));
               }
            }
         }

         return newPopup;
      }
      else
      {
         Debug.LogError($"Failed to show popup with id: {popupId}");
         return null;
      }
   }

   public UIPopupComponent GetCurrentlyShownPopup()
   {
      return _activePopup.Item2;
   }

   //stashing popups should be used when you want to keep the existing popup queue, but hide them all temporarily
   public void StashAllPopups()
   {
      if (!_isCurrentlyStashed)
      {
         _isCurrentlyStashed = true;
         string activePopupId = _activePopup.Item1;
         if (!string.IsNullOrEmpty(activePopupId))
         {
            _activePopup.Item2.OnPopupHide();
            OnPopupHidden?.Invoke(activePopupId);
            _popupQueue.AddFirst((_activePopup.Item1, _activePopup.Item2));
            _activePopup = ("", null);
         }
      }
   }

   public void UnstashAllPopups()
   {
      if (_isCurrentlyStashed)
      {
         _isCurrentlyStashed = false;
         
         ShowNextQueuedPopup();
      }
   }

   public void HideActivePopup()
   {
      if (_activePopup.Item2 != null)
      {
         HidePopup(_activePopup.Item1);
      }
   }
   
   public void HidePopup(string popupId)
   {
      (string, UIPopupComponent) uiPopup = _popupQueue.FirstOrDefault(popup => popup.Item1 == popupId);

      if (uiPopup.Item2 != null)
      {
         _popupQueue.Remove(uiPopup);
         
         uiPopup.Item2.OnPopupHide();
         OnPopupHidden?.Invoke(popupId);

         if (!_isCurrentlyStashed)
         {
            ShowNextQueuedPopup();
         }
      }
      else if (_activePopup.Item1 == popupId)
      {
         _activePopup.Item2.OnPopupHide();
         _activePopup = ("", null);
         OnPopupHidden?.Invoke(popupId);
         

         if (!_isCurrentlyStashed)
         {
            ShowNextQueuedPopup();
         }
      }
   }

   private void ShowNextQueuedPopup()
   {
      if (_popupQueue.Count > 0)
      {
         string nextPopupId = _popupQueue.First.Value.Item1;
         
         _popupQueue.RemoveFirst();
         
         ShowPopup(nextPopupId, PanelShowBehaviour.HIDE_PREVIOUS);
      }
   }

   public bool IsPopupShowing()
   {
      return _activePopup.Item2 != null;
   }
   
   public bool IsPopupShowing(string popupName)
   {
      return _activePopup.Item1 == popupName;
   }
   
   private void RegisterCallbackToInput(UnityAction<UIInputChannel.UIInputChannelCallbackArgs> callback, UIInputType inputType)
   {
      switch (inputType)
      {
         case UIInputType.NAVIGATE:
            inputChannel.NavigateEvent += callback;
            break;
         case UIInputType.NAVIGATE_UP:
            inputChannel.NavigateUpEvent += callback;
            break;
         case UIInputType.NAVIGATE_DOWN:
            inputChannel.NavigateDownEvent += callback;
            break;
         case UIInputType.NAVIGATE_LEFT:
            inputChannel.NavigateLeftEvent += callback;
            break;
         case UIInputType.NAVIGATE_RIGHT:
            inputChannel.NavigateRightEvent += callback;
            break;
         case UIInputType.MOUSE_MOVE:
            inputChannel.MouseMoveEvent += callback;
            break;
         case UIInputType.MOUSE_SELECT:
            inputChannel.MouseSelectEvent += callback;
            break;
         case UIInputType.SELECT:
            inputChannel.SelectEvent += callback;
            break;
         case UIInputType.BACK:
            inputChannel.BackEvent += callback;
            break;
         case UIInputType.PAUSE:
            inputChannel.PauseEvent += callback;
            break;
         case UIInputType.UNPAUSE:
            inputChannel.UnpauseEvent += callback;
            break;
         case UIInputType.QUIT:
            inputChannel.QuitEvent += callback;
            break;
         default:
            break;
      }
   }
   
   private void DeregisterCallbackFromInput(UnityAction<UIInputChannel.UIInputChannelCallbackArgs> callback, UIInputType inputType)
   {
      switch (inputType)
      {
         case UIInputType.NAVIGATE:
            inputChannel.NavigateEvent -= callback;
            break;
         case UIInputType.NAVIGATE_UP:
            inputChannel.NavigateUpEvent -= callback;
            break;
         case UIInputType.NAVIGATE_DOWN:
            inputChannel.NavigateDownEvent -= callback;
            break;
         case UIInputType.NAVIGATE_LEFT:
            inputChannel.NavigateLeftEvent -= callback;
            break;
         case UIInputType.NAVIGATE_RIGHT:
            inputChannel.NavigateRightEvent -= callback;
            break;
         case UIInputType.MOUSE_MOVE:
            inputChannel.MouseMoveEvent -= callback;
            break;
         case UIInputType.MOUSE_SELECT:
            inputChannel.MouseSelectEvent -= callback;
            break;
         case UIInputType.SELECT:
            inputChannel.NavigateDownEvent -= callback;
            break;
         case UIInputType.BACK:
            inputChannel.BackEvent -= callback;
            break;
         case UIInputType.PAUSE:
            inputChannel.PauseEvent -= callback;
            break;
         case UIInputType.UNPAUSE:
            inputChannel.UnpauseEvent -= callback;
            break;
         case UIInputType.QUIT:
            inputChannel.QuitEvent -= callback;
            break;
         default:
            break;
      }
   }
}

#endif
