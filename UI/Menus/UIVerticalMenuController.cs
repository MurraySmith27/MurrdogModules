using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UltEvents;
using Unity.VisualScripting;
#if USING_FMOD
using FMODUnity;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;


[Serializable]
public class UIMenuAnimationIntParameter
{
    public string name;
    public int value;
    
    public void SetAnimatorParameter(Animator animator)
    {
        if (animator.parameters.FirstOrDefault(parameter => { return parameter.name == name; }) == null)
        {
            Debug.LogError($"Error in menu navigation! No such parameter named {name} in animator: {animator} exists!");
            return;
        }

        animator.SetInteger(name, value);
    }
}


public class UIVerticalMenuController : MonoBehaviour
{
    [Serializable]
    public class UIVerticalMenuControllerButtonEvent : UltEvent {}
    
    #region Inspector Variables
    
#if USING_FMOD
    [SerializeField] private StudioEventEmitter menuSelectSound;
    [SerializeField] private StudioEventEmitter menuNavigateSound;
#else
    [SerializeField] private AudioSource menuSelectSound;
    [SerializeField] private AudioSource menuNavigateSound;
#endif
    
    [Header("Animation")]
    
    [SerializeField] private List<Animator> buttons;

    [SerializeField] private List<UIMenuAnimationIntParameter> onNavigateToAnimationParameters;
    
    [SerializeField] private List<UIMenuAnimationIntParameter> onNavigateFromAnimationParameters;
    
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActionAsset;
    
    [SerializeField] private string navigateActionName;
    
    [SerializeField] private string selectActionName;

    [SerializeField] private string mouseNavigateActionName;
    
    [SerializeField] private string mouseSelectActionName;
    
    [Header("Actions")]
    [SerializeField]
    private List<UIVerticalMenuControllerButtonEvent> OnSelectedEventsPerButton = new List<UIVerticalMenuControllerButtonEvent>();
    
    #endregion

    #region Member Variables
    
    private int _currentlySelectedMenuButton = -1;
    
    private InputAction _selectAction;
    
    private InputAction _navigateAction;
    
    private InputAction _mouseNavigateAction;
    
    private InputAction _mouseSelectAction;
    
    private bool _freezeNavigation;

    private bool _freezeNavigationExternal;
    
    #endregion
    
    
    private void Awake()
    {
        _freezeNavigation = false;

        _freezeNavigationExternal = false;

        _navigateAction = inputActionAsset.FindAction(navigateActionName);
        _selectAction = inputActionAsset.FindAction(selectActionName);
        _mouseNavigateAction = inputActionAsset.FindAction(mouseNavigateActionName);
        _mouseSelectAction = inputActionAsset.FindAction(mouseSelectActionName);
        
        EnableActions();
    }

    private void EnableActions()
    {
        if (_navigateAction != null)
        {
            _navigateAction.performed -= OnNavigatePerformed;
            _navigateAction.performed += OnNavigatePerformed;
            _navigateAction.Enable();
        }

        if (_selectAction != null)
        {
            _selectAction.performed -= OnSelectPerformed;
            _selectAction.performed += OnSelectPerformed;
            _selectAction.Enable();
        }
        
        if (_mouseNavigateAction != null)
        {
            _mouseNavigateAction.performed -= OnMouseNavigatePerformed;
            _mouseNavigateAction.performed += OnMouseNavigatePerformed;
            _mouseNavigateAction.Enable();
        }

        if (_mouseSelectAction != null)
        {
            _mouseSelectAction.performed -= OnMouseSelectPerformed;
            _mouseSelectAction.performed += OnMouseSelectPerformed;
            _mouseSelectAction.Enable();
        }
    }
    
    private void DisableActions()
    {
        if (_navigateAction != null)
        {
            _navigateAction.performed -= OnNavigatePerformed;
            _navigateAction.Disable();
        }

        if (_selectAction != null)
        {
            _selectAction.performed -= OnSelectPerformed;
            _selectAction.Disable();
        }
        
        if (_mouseNavigateAction != null)
        {
            _mouseNavigateAction.performed -= OnMouseNavigatePerformed;
            _mouseNavigateAction.Disable();
        }

        if (_mouseSelectAction != null)
        {
            _mouseSelectAction.performed -= OnMouseSelectPerformed;
            _mouseSelectAction.Disable();
        }
    }
    
    

    private void OnNavigatePerformed(InputAction.CallbackContext ctx)
    {
        if (!_freezeNavigation && !_freezeNavigationExternal)
        {
            if (_currentlySelectedMenuButton == -1)
            {
                _currentlySelectedMenuButton = 0;
            }
            else
            {
                Vector2 moveDirection = _navigateAction.ReadValue<Vector2>();

                if (moveDirection.magnitude == 0)
                {
                    return;
                }
                else if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
                {
                    if (moveDirection.x > 0)
                    {
                        //move right

                    }
                    else
                    {
                        //move left
                    }
                }
                else
                {
                    if (moveDirection.y > 0)
                    {
                        if (_currentlySelectedMenuButton != 0)
                        {
                            _currentlySelectedMenuButton--;
                            if (menuNavigateSound != null)
                            {
                                menuNavigateSound.Play();
                            }

                            StartCoroutine(ButtonNavigationCooldown());
                        }
                    }
                    else
                    {
                        if (_currentlySelectedMenuButton != buttons.Count-1)
                        {
                            _currentlySelectedMenuButton++;
                            if (menuNavigateSound != null)
                            {
                                menuNavigateSound.Play();
                            }
                            StartCoroutine(ButtonNavigationCooldown());
                        }
                    }
                }
            }

            UpdateMenuButtons();
        }
    }
    
    private IEnumerator ButtonNavigationCooldown()
    {
        _freezeNavigation = true;
        yield return null;
        _freezeNavigation = false;
    }

    private void UpdateMenuButtons()
    {
        for (int i = 0; i < buttons.Count; i++) 
        {
            if (i == _currentlySelectedMenuButton)
            {
                onNavigateToAnimationParameters[i].SetAnimatorParameter(buttons[i]);
            }
            else
            {
                onNavigateFromAnimationParameters[i].SetAnimatorParameter(buttons[i]);
            }
        }
    }
    
    private void OnMouseNavigatePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 mousePos = _mouseNavigateAction.ReadValue<Vector2>();
        bool foundMenuButton = false;

        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform buttonRectTransform = buttons[i].transform as RectTransform;
            Vector2 localMousePosition = buttonRectTransform.InverseTransformPoint(mousePos);
            if (buttonRectTransform.rect.Contains(localMousePosition) && _currentlySelectedMenuButton != i)
            {
                _currentlySelectedMenuButton = i;
                foundMenuButton = true;
                if (menuNavigateSound != null)
                {
                    menuNavigateSound.Play();
                }
                break;
            }
        }

        if (foundMenuButton)
        {
            UpdateMenuButtons();
        }
    }

    private void OnSelectPerformed(InputAction.CallbackContext ctx)
    {

        if (_currentlySelectedMenuButton == -1)
        {
            return;
        }

        if (menuSelectSound != null)
        {
            menuSelectSound.Play();
        } 
        OnSelectedEventsPerButton[_currentlySelectedMenuButton].Invoke();
    }


    private void OnMouseSelectPerformed(InputAction.CallbackContext ctx)
    {
        OnMouseNavigatePerformed(ctx);
        Vector2 mousePos = _mouseNavigateAction.ReadValue<Vector2>();
        bool foundMenuButton = false;

        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform buttonRectTransform = buttons[i].transform as RectTransform;
            Vector2 localMousePosition = buttonRectTransform.InverseTransformPoint(mousePos);
            if (buttonRectTransform.rect.Contains(localMousePosition))
            {
                foundMenuButton = true;
                break;
            }
        }

        if (foundMenuButton)
        {
            OnSelectPerformed(ctx);
        }
    }

    public void SetFreezeNavigation(bool isFrozen)
    {
        _freezeNavigationExternal = isFrozen;
    }

    private void OnEnable()
    {
        EnableActions();
        
        _freezeNavigation = false;
        UpdateMenuButtons();
    }
    
    private void OnDisable()
    {
        DisableActions();
    }
}
