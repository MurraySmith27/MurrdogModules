using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "UIInputChannel", menuName = "Orpheus/UIInputChannel", order = 1)]
public class OrpheusUIInputChannel : UIInputChannel, UIInputActions.IUIActions
{
    private UIInputActions _uiInputActions;

    private Vector2 _currentMousePosition;
    
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
        if (ctx.performed)
        {
            base.InvokeRightMouseDownEvent(_currentMousePosition);
        }
        else if (ctx.canceled)
        {
            base.InvokeRightMouseUpEvent(_currentMousePosition);
        }
    }
    
    public void OnLeftMouseClick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            base.InvokeLeftMouseDownEvent(_currentMousePosition);
        }
        else if (ctx.canceled)
        {
            base.InvokeLeftMouseUpEvent(_currentMousePosition);
        }
    }

    public void OnLeftMouseHeld(InputAction.CallbackContext ctx)
    {
        Debug.LogError($"Left Mouse Held Event! {ctx.performed}, {ctx.canceled}");
        
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
        _currentMousePosition = ctx.ReadValue<Vector2>();
        base.InvokeMouseMoveEvent(_currentMousePosition);
    }

    public void OnMouseScroll(InputAction.CallbackContext ctx)
    {
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
