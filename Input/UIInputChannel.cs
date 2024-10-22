using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UIInputChannel : ScriptableObject
{
    public event UnityAction NavigateUpEvent;
    public event UnityAction NavigateDownEvent;
    public event UnityAction NavigateLeftEvent;
    public event UnityAction NavigateRightEvent;

    public event UnityAction<Vector2> NavigateEvent;
    
    public event UnityAction SelectEvent;

    public event UnityAction<Vector2> MouseSelectEvent;

    public event UnityAction<Vector2> MouseMoveEvent;
    
    public event UnityAction BackEvent;

    public event UnityAction PauseEvent;

    public event UnityAction UnpauseEvent;

    public event UnityAction QuitEvent;
    
    protected void InvokeNavigateUpEvent()
    {
        NavigateUpEvent?.Invoke();
    }
    
    protected void InvokeNavigateDownEvent()
    {
        NavigateDownEvent?.Invoke();
    }
    
    protected void InvokeNavigateLeftEvent()
    {
        NavigateLeftEvent?.Invoke();
    }
    
    protected void InvokeNavigateRightEvent()
    {
        NavigateRightEvent?.Invoke();
    }
    
    protected void InvokeNavigateEvent(Vector2 input)
    {
        NavigateEvent?.Invoke(input);
    }
    
    protected void InvokeSelectEvent()
    {
        SelectEvent?.Invoke();
    }
    
    protected void InvokeMouseSelectEvent(Vector2 input)
    {
        MouseSelectEvent?.Invoke(input);
    }
    
    protected void InvokeMouseMoveEvent(Vector2 input)
    {
        MouseMoveEvent?.Invoke(input);
    }
    
    protected void InvokeBackEvent()
    {
        BackEvent?.Invoke();
    }

    protected void InvokePauseEvent()
    {
        PauseEvent?.Invoke();
    }
    
    protected void InvokeUnpauseEvent()
    {
        UnpauseEvent?.Invoke();   
    }
    
    protected void InvokeQuitEvent()
    {
        QuitEvent?.Invoke();
    }
}
