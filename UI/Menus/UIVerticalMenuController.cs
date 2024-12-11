using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
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
using Vector2 = UnityEngine.Vector2;


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
    [SerializeField] private UIInputChannel uiInputChannel;

    [SerializeField] private float verticalNavigationInputCooldown = 0.1f;
    
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
    
    #endregion
    
    
    private void Awake()
    {
        _freezeNavigation = false;
        
        EnableActions();
    }

    private void EnableActions()
    {
        uiInputChannel.NavigateUpEvent -= OnNavigateUp;
        uiInputChannel.NavigateUpEvent += OnNavigateUp;
        
        uiInputChannel.NavigateDownEvent -= OnNavigateDown;
        uiInputChannel.NavigateDownEvent += OnNavigateDown;

        uiInputChannel.SelectEvent -= OnSelectPerformed;
        uiInputChannel.SelectEvent += OnSelectPerformed;

        uiInputChannel.MouseMoveEvent -= OnMouseNavigatePerformed;
        uiInputChannel.MouseMoveEvent += OnMouseNavigatePerformed;

        uiInputChannel.MouseSelectEvent -= OnMouseSelectPerformed;
        uiInputChannel.MouseSelectEvent += OnMouseSelectPerformed;
    }
    
    private void DisableActions()
    {
        uiInputChannel.NavigateUpEvent -= OnNavigateUp;
        
        uiInputChannel.NavigateDownEvent -= OnNavigateDown;

        uiInputChannel.SelectEvent -= OnSelectPerformed;

        uiInputChannel.MouseMoveEvent -= OnMouseNavigatePerformed;

        uiInputChannel.MouseSelectEvent -= OnMouseSelectPerformed;
    }

    private void OnNavigateUp(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        if (!_freezeNavigation)
        {
            if (_currentlySelectedMenuButton == -1)
            {
                _currentlySelectedMenuButton = 0;
            }
            else if (_currentlySelectedMenuButton != 0)
            {
                Debug.Log($"setting currently selected menu button to: {_currentlySelectedMenuButton - 1}");
                _currentlySelectedMenuButton--;
                if (menuNavigateSound != null)
                {
                    menuNavigateSound.Play();
                }

                StartCoroutine(ButtonNavigationCooldown());
            }
            
            UpdateMenuButtons();
        }
    }
    
    private void OnNavigateDown(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        if (!_freezeNavigation)
        {
            if (_currentlySelectedMenuButton == -1)
            {
                _currentlySelectedMenuButton = 0;
            }
            else if (_currentlySelectedMenuButton != buttons.Count-1)
            {
                Debug.Log($"setting currently selected menu button to: {_currentlySelectedMenuButton + 1}");
                _currentlySelectedMenuButton++;
                if (menuNavigateSound != null)
                {
                    menuNavigateSound.Play();
                }
                StartCoroutine(ButtonNavigationCooldown());
            }
            
            UpdateMenuButtons();
        }
    }
    
    

    private void OnNavigatePerformed(InputAction.CallbackContext ctx)
    {
        if (!_freezeNavigation)
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
        yield return new WaitForSecondsRealtime(verticalNavigationInputCooldown);
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
    
    private void OnMouseNavigatePerformed(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        Vector2 mousePosition = (Vector2)args.vector2Arg;
        bool foundMenuButton = false;

        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform buttonRectTransform = buttons[i].transform as RectTransform;
            Vector2 localMousePosition = buttonRectTransform.InverseTransformPoint(mousePosition);
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

    private void OnSelectPerformed(UIInputChannel.UIInputChannelCallbackArgs args)
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


    private void OnMouseSelectPerformed(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        Vector2 mousePosition = (Vector2)args.vector2Arg;
        OnMouseNavigatePerformed(args);
        bool foundMenuButton = false;

        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform buttonRectTransform = buttons[i].transform as RectTransform;
            Vector2 localMousePosition = buttonRectTransform.InverseTransformPoint(mousePosition);
            if (buttonRectTransform.rect.Contains(localMousePosition))
            {
                foundMenuButton = true;
                break;
            }
        }

        if (foundMenuButton)
        {
            OnSelectPerformed(args);
        }
    }

    public void SetFreezeNavigation(bool isFrozen)
    {
        if (isFrozen)
        {
            DisableActions();
        }
        else
        {
            EnableActions();
        }
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
