using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "UIInputChannel", menuName = "Orpheus/UIInputChannel", order = 1)]
public class OrpheusUIInputChannel : UIInputChannel, UIInputActions.IUIActions
{
    private UIInputActions _uiInputActions;

    private Vector2 _currentMousePosition;

    private float _lastLeftClickTime;
    private float _lastRightClickTime;
    
    private float _doubleClickDistanceThreshold = 0.001f;

    private float _mouseClickTimeThreshold = 0.3f;
    
    private void OnEnable()
    {
        if (_uiInputActions == null)
        {
            _uiInputActions = new UIInputActions();
            _uiInputActions.UI.SetCallbacks(this);
        }
        
        ToggleEnabled(true);
    }

    private void OnDisable()
    {
        _uiInputActions.UI.Disable();
    }
    
    public void ToggleEnabled(bool enabled)
    {
        if (enabled)
        {
            _uiInputActions.UI.Enable();
        }
        else
        {
            _uiInputActions.UI.Disable();
        }
    }
    
    public void OnRightMouseClick(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (ctx.performed)
        {
            _lastRightClickTime = Time.unscaledTime;
            base.InvokeRightMouseDownEvent(_currentMousePosition);
        }
        else if (ctx.canceled)
        {
            if (Time.unscaledTime - _lastRightClickTime <= _mouseClickTimeThreshold)
            {
                base.InvokeMouseRightClickEvent(_currentMousePosition);
            }
            
            base.InvokeRightMouseUpEvent(_currentMousePosition);
        }
    }
    
    public void OnLeftMouseClick(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (ctx.performed)
        {
            _lastLeftClickTime = Time.unscaledTime;
            base.InvokeLeftMouseDownEvent(_currentMousePosition);
        }
        else if (ctx.canceled)
        {
            if (Time.unscaledTime - _lastLeftClickTime <= _mouseClickTimeThreshold)
            {
                base.InvokeMouseLeftClickEvent(_currentMousePosition);
            }
            base.InvokeLeftMouseUpEvent(_currentMousePosition);
        }
    }
    
    public void OnLeftMouseDoubleClick(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (ctx.performed)
        {
            base.InvokeLeftMouseDoubleClickEvent(_currentMousePosition);
        }
    }

    public void OnLeftMouseHeld(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            base.InvokeLeftMouseHeldEvent(_currentMousePosition);
        }
        else if (ctx.canceled)
        {
            base.InvokeLeftMouseUpEvent(_currentMousePosition);
        }
    }
    
    public void OnRightMouseHeld(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (ctx.performed)
        {
            base.InvokeRightMouseHeldEvent(_currentMousePosition);
        }
        else if (ctx.canceled)
        {
            base.InvokeRightMouseUpEvent(_currentMousePosition);
        }
    }
    
    public void OnMouseNavigation(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            base.InvokeMouseMoveEvent(new Vector2(-1, -1));
            _currentMousePosition = new Vector2(-1, -1);
            return;
        }
        
        _currentMousePosition = ctx.ReadValue<Vector2>();
        base.InvokeMouseMoveEvent(_currentMousePosition);
    }

    public void OnMouseScroll(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        Vector2 input = ctx.ReadValue<Vector2>();

        if (input.x != 0)
        {
            base.InvokeMouseHorizontalScrollEvent(input.x);
        }

        if (input.y != 0)
        {
            base.InvokeMouseVerticalScrollEvent(input.y);
        }
    }
}
