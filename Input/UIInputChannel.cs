using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UIInputChannel : ScriptableObject
{
    public struct UIInputChannelCallbackArgs
    {
        public readonly float? floatArg;
        public readonly Vector2? vector2Arg;

        public UIInputChannelCallbackArgs(Vector2 vector2Arg = new Vector2(), float floatArg = 0f)
        {
            this.floatArg = floatArg;
            this.vector2Arg = vector2Arg;
        }
    }
    
    public event UnityAction<UIInputChannelCallbackArgs> NavigateUpEvent;
    public event UnityAction<UIInputChannelCallbackArgs> NavigateDownEvent;
    public event UnityAction<UIInputChannelCallbackArgs> NavigateLeftEvent;
    public event UnityAction<UIInputChannelCallbackArgs> NavigateRightEvent;

    public event UnityAction<UIInputChannelCallbackArgs> NavigateEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> SelectEvent;

    public event UnityAction<UIInputChannelCallbackArgs> MouseSelectEvent;

    public event UnityAction<UIInputChannelCallbackArgs> LeftMouseDownEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> LeftMouseDoubleClickEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> RightMouseDownEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> LeftMouseHeldEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> RightMouseHeldEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> LeftMouseUpEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> LeftMouseClickEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> RightMouseUpEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> RightMouseClickEvent;

    public event UnityAction<UIInputChannelCallbackArgs> MouseMoveEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> MouseVerticalScrollEvent;
    
    public event UnityAction<UIInputChannelCallbackArgs> MouseHorizontalScrollEvent;
    
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
        NavigateEvent?.Invoke(new(vector2Arg: input));
    }
    
    protected void InvokeSelectEvent()
    {
        SelectEvent?.Invoke(new());
    }
    
    protected void InvokeMouseSelectEvent(Vector2 input)
    {
        MouseSelectEvent?.Invoke(new(vector2Arg: input));
    }

    protected void InvokeLeftMouseDownEvent(Vector2 input)
    {
        LeftMouseDownEvent?.Invoke(new(vector2Arg: input));
    }

    protected void InvokeLeftMouseDoubleClickEvent(Vector2 input)
    {
        LeftMouseDoubleClickEvent?.Invoke(new(vector2Arg: input));
    }
    
    protected void InvokeRightMouseDownEvent(Vector2 input)
    {
        RightMouseDownEvent?.Invoke(new(vector2Arg: input));
    }

    protected void InvokeLeftMouseHeldEvent(Vector2 input)
    {
        LeftMouseHeldEvent?.Invoke(new (input));
    }
    
    protected void InvokeRightMouseHeldEvent(Vector2 input)
    {
        RightMouseHeldEvent?.Invoke(new (input));
    }
    
    protected void InvokeLeftMouseUpEvent(Vector2 input)
    {
        LeftMouseUpEvent?.Invoke(new(vector2Arg: input));
    }

    protected void InvokeMouseLeftClickEvent(Vector2 input)
    {
        LeftMouseClickEvent?.Invoke(new(vector2Arg: input));
    }
    
    protected void InvokeRightMouseUpEvent(Vector2 input)
    {
        RightMouseUpEvent?.Invoke(new(vector2Arg: input));
    }
    
    protected void InvokeMouseRightClickEvent(Vector2 input)
    {
        RightMouseClickEvent?.Invoke(new(vector2Arg: input));
    }
    
    protected void InvokeMouseMoveEvent(Vector2 input)
    {
        MouseMoveEvent?.Invoke(new(vector2Arg: input));
    }

    protected void InvokeMouseVerticalScrollEvent(float input)
    {
        MouseVerticalScrollEvent?.Invoke(new (floatArg: input));
    }

    protected void InvokeMouseHorizontalScrollEvent(float input)
    {
        MouseHorizontalScrollEvent?.Invoke(new (floatArg: input));
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
