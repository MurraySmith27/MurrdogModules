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
        
    }
    
    public void OnLeftMouseClick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            base.InvokeMouseDownEvent(_currentMousePosition);
        }
        else if (ctx.canceled)
        {
            base.InvokeMouseUpEvent(_currentMousePosition);
        }
    }
    
    public void OnMouseNavigation(InputAction.CallbackContext ctx)
    {
        _currentMousePosition = ctx.ReadValue<Vector2>();
        base.InvokeMouseMoveEvent(_currentMousePosition);
    }
}
