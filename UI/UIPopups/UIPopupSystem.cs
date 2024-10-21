using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum PanelShowBehaviour
{
   KEEP_PREVIOUS,
   HIDE_PREVIOUS
}

public class UIPopupSystem : Singleton<UIPopupSystem>
{
   [SerializeField] private InputActionAsset inputActionAsset;
   
   public List<UIPopup> uiPopups;

   private LinkedList<(string, UIPopupComponent)> _popupQueue = new();

   private (string, UIPopupComponent) _activePopup;

   private Dictionary<string, UIPopupComponent> _popupInstancePool = new();

   private List<(InputAction, Action<InputAction.CallbackContext>)> _showPopupActions = new();
   
   private List<(InputAction, Action<InputAction.CallbackContext>)> _hidePopupActions = new();

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
      
      if (inputActionAsset != null)
      {
         foreach (UIPopup uiPopup in uiPopups)
         {
            if (!uiPopup.excludedSceneNames.Contains(currentSceneName))
            {
               InputAction showInputAction = inputActionAsset.FindAction(uiPopup.showPopupActionName);
               InputAction hideInputAction = inputActionAsset.FindAction(uiPopup.hidePopupActionName);
               
               string popupId = uiPopup.id; 

               if (showInputAction != null)
               {
                  Action<InputAction.CallbackContext> onShowPopupAction = (InputAction.CallbackContext ctx) =>
                  {
                     ShowPopup(popupId);
                  };
                  
                  showInputAction.performed += onShowPopupAction;
                  showInputAction.Enable();
                  
                  _showPopupActions.Add((showInputAction, onShowPopupAction));
               }
               
               if (hideInputAction != null)
               {
                  Action<InputAction.CallbackContext> onHidePopupAction = (InputAction.CallbackContext ctx) =>
                  {
                     HidePopup(popupId);
                  };
                  
                  hideInputAction.performed += onHidePopupAction;
                  hideInputAction.Enable();
                  
                  _hidePopupActions.Add((hideInputAction, onHidePopupAction));
               }
            }
         }
      }
      else
      {
         Debug.LogError("UIPopupSystem has not been assigned an InputActionAsset! Popup show and hide actions might not work.");
      }
   }

   private void ResetInputActions()
   {
      foreach ((InputAction, Action<InputAction.CallbackContext>) showPopupAction in _showPopupActions)
      {
         showPopupAction.Item1.performed -= showPopupAction.Item2;
      }
      
      foreach ((InputAction, Action<InputAction.CallbackContext>) hidePopupAction in _hidePopupActions)
      {
         hidePopupAction.Item1.performed -= hidePopupAction.Item2;
      }
      
      _showPopupActions.Clear();
      _hidePopupActions.Clear();
   }

   public void ShowPopup(string popupId, PanelShowBehaviour showBehaviour = PanelShowBehaviour.KEEP_PREVIOUS)
   {
      UIPopup uiPopup = uiPopups.FirstOrDefault(popup => popup.id == popupId);

      if (uiPopup != null)
      {
         //see if we've already queued this popup.
         if (_popupQueue.FirstOrDefault(popup => popup.Item1 == popupId).Item2 != null)
         {
            return;
         }

         if (!_popupInstancePool.ContainsKey(popupId))
         {
            uiPopup.prefab.SetActive(false);
            _popupInstancePool.Add(popupId, Instantiate(uiPopup.prefab, transform).GetComponent<UIPopupComponent>());
         }
         
         UIPopupComponent newPopup = _popupInstancePool[popupId];

         if (_popupQueue.Count == 0 && _activePopup.Item2 == null)
         {
            foreach ((InputAction, Action<InputAction.CallbackContext>) hidePopupAction in _hidePopupActions)
            {
               hidePopupAction.Item1.Enable();
            }
         
            _activePopup = (popupId, newPopup.GetComponent<UIPopupComponent>());
         
            newPopup.OnPopupShow();
         }
         else
         {
            if (showBehaviour == PanelShowBehaviour.HIDE_PREVIOUS)
            {
               if (_activePopup.Item2 != null)
               {
                  _activePopup.Item2.OnPopupHide();

                  _popupQueue.AddFirst(_activePopup);

                  _activePopup = ("", null);
               }
               
               foreach ((InputAction, Action<InputAction.CallbackContext>) hidePopupAction in _hidePopupActions)
               {
                  hidePopupAction.Item1.Enable();
               }
         
               _activePopup = (popupId, newPopup.GetComponent<UIPopupComponent>());
         
               newPopup.OnPopupShow();
            }
            else
            {
               _popupQueue.AddLast((popupId, newPopup.GetComponent<UIPopupComponent>()));
            }
         }
      }
      else
      {
         Debug.LogError($"Failed to show popup with id: {popupId}");
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
         
         foreach ((InputAction, Action<InputAction.CallbackContext>) showPopupAction in _showPopupActions)
         {
            showPopupAction.Item1.Enable();
         }
         
         uiPopup.Item2.OnPopupHide();
         
         ShowNextQueuedPopup();
      }
      else if (_activePopup.Item1 == popupId)
      {
         foreach ((InputAction, Action<InputAction.CallbackContext>) showPopupAction in _showPopupActions)
         {
            showPopupAction.Item1.Enable();
         }
         
         _activePopup.Item2.OnPopupHide();
         
         _activePopup = ("", null);

         ShowNextQueuedPopup();
      }
   }

   private void ShowNextQueuedPopup()
   {
      if (_popupQueue.Count > 0)
      {
         string nextPopupId = _popupQueue.First.Value.Item1;
         
         _popupQueue.RemoveFirst();
         
         ShowPopup(nextPopupId);
      }
   }

   public bool IsPopupShowing()
   {
      return _activePopup.Item2 != null;
   }

   private void OnEnable()
   {
      foreach ((InputAction, Action<InputAction.CallbackContext>) showPopupAction in _showPopupActions)
      {
         showPopupAction.Item1.Enable();
      }
      
      foreach ((InputAction, Action<InputAction.CallbackContext>) hidePopupAction in _hidePopupActions)
      {
         hidePopupAction.Item1.Enable();
      }
   }

   private void OnDisable()
   {
      foreach ((InputAction, Action<InputAction.CallbackContext>) showPopupAction in _showPopupActions)
      {
         showPopupAction.Item1.Disable();
      }
      
      foreach ((InputAction, Action<InputAction.CallbackContext>) hidePopupAction in _hidePopupActions)
      {
         hidePopupAction.Item1.Disable();
      }
   }
}
