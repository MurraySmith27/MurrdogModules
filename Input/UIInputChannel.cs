using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UIInputChannel : ScriptableObject
{
    public struct UIInputChannelCallbackArgs
    {
        public readonly Vector2? vector2Arg;
        
        public UIInputChannelCallbackArgs(Vector2 arg)
        {
            vector2Arg = arg;
        }
    }
    
    public event UnityAction<UIInputChannelCallbackArgs> NavigateUpEvent;
    public event UnityAction<UIInputChannelCallbackArgs> NavigateDownEvent;
    public event UnityAction<UIInputChannelCallbackArgs> NavigateLeftEvent;
    public event UnityAction<UIInputChannelCallbackArgs> NavigateRightEvent;

    public event UnityAction<UIInputChannelCallbackArgs> NavigateEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> SelectEvent;

    public event UnityAction<UIInputChannelCallbackArgs> MouseSelectEvent;

    public event UnityAction<UIInputChannelCallbackArgs> MouseDownEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> MouseUpEvent;

    public event UnityAction<UIInputChannelCallbackArgs> MouseMoveEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> BackEvent;

    public event UnityAction<UIInputChannelCallbackArgs> PauseEvent;

    public event UnityAction<UIInputChannelCallbackArgs> UnpauseEvent;

    public event UnityAction<UIInputChannelCallbackArgs> QuitEvent;
    
    protected void InvokeNavigateUpEvent()
    {
        NavigateUpEvent?.Invoke(new());
    }
    
    protected void InvokeNavigateDownEvent()
    {
        NavigateDownEvent?.Invoke(new());
    }
    
    protected void InvokeNavigateLeftEvent()
    {
        NavigateLeftEvent?.Invoke(new());
    }
    
    protected void InvokeNavigateRightEvent()
    {
        NavigateRightEvent?.Invoke(new());
    }
    
    protected void InvokeNavigateEvent(Vector2 input)
    {
        NavigateEvent?.Invoke(new(input));
    }
    
    protected void InvokeSelectEvent()
    {
        SelectEvent?.Invoke(new());
    }
    
    protected void InvokeMouseSelectEvent(Vector2 input)
    {
        MouseSelectEvent?.Invoke(new(input));
    }

    protected void InvokeMouseDownEvent(Vector2 input)
    {
        MouseDownEvent?.Invoke(new(input));
    }
    
    protected void InvokeMouseUpEvent(Vector2 input)
    {
        MouseUpEvent?.Invoke(new(input));
    }
    
    protected void InvokeMouseMoveEvent(Vector2 input)
    {
        MouseMoveEvent?.Invoke(new(input));
    }
    
    protected void InvokeBackEvent()
    {
        BackEvent?.Invoke(new());
    }

    protected void InvokePauseEvent()
    {
        PauseEvent?.Invoke(new());
    }
    
    protected void InvokeUnpauseEvent()
    {
        UnpauseEvent?.Invoke(new());   
    }
    
    protected void InvokeQuitEvent()
    {
        QuitEvent?.Invoke(new());
    }
}
