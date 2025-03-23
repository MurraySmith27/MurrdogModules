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

    private int _uiLayer;
    
    private void OnEnable()
    {
        if (_uiInputActions == null)
        {
            _uiInputActions = new UIInputActions();
            _uiInputActions.UI.SetCallbacks(this);
        }
        
        ToggleEnabled(true);
        
        _uiLayer = LayerMask.NameToLayer("MainUI");
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
    
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == _uiLayer)
                return true;
        }
        return false;
    }
    
    public void OnRightMouseClick(InputAction.CallbackContext ctx)
    {
        if (IsPointerOverUIElement())
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
        if (IsPointerOverUIElement())
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
        if (IsPointerOverUIElement())
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
        if (IsPointerOverUIElement())
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
        if (IsPointerOverUIElement())
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
        if (IsPointerOverUIElement())
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
