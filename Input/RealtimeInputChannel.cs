using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class RealtimeInputChannel : ScriptableObject
{
    public struct RealtimeInputChannelCallbackArgs
    {
        public readonly float? floatArg;
        public readonly Vector2? vector2Arg;

        public RealtimeInputChannelCallbackArgs(Vector2 vector2Arg = new Vector2(), float floatArg = 0f)
        {
            this.floatArg = floatArg;
            this.vector2Arg = vector2Arg;
        }
    }

    public event UnityAction<RealtimeInputChannelCallbackArgs> MoveEvent;
    
    public event UnityAction<RealtimeInputChannelCallbackArgs> SprintStartEvent;
    public event UnityAction<RealtimeInputChannelCallbackArgs> SprintEndEvent;
    
    public event UnityAction<RealtimeInputChannelCallbackArgs> SelectEvent;
    
    public event UnityAction<RealtimeInputChannelCallbackArgs> BackEvent;
    
    protected void InvokeMoveEvent(Vector2 input)
    {
        MoveEvent?.Invoke(new(vector2Arg: input));
    }

    protected void InvokeSprintStartEvent()
    {
        SprintStartEvent?.Invoke(new());
    }
    
    protected void InvokeSprintEndEvent()
    {
        SprintEndEvent?.Invoke(new());
    }
    
    protected void InvokeSelectEvent()
    {
        SelectEvent?.Invoke(new());
    }
    
    protected void InvokeBackEvent()
    {
        BackEvent?.Invoke(new());
    }

}
