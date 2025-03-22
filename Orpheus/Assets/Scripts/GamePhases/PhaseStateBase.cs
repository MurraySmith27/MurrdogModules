using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseStateBase
{
    public virtual void StateEnter(PhaseStateMachine context, Action onEnterComplete)
    {
        onEnterComplete?.Invoke();
    }
    
    public virtual void StateUpdate(PhaseStateMachine context)
    {
        
    }

    public virtual void StateExit(PhaseStateMachine context, Action onExitComplete)
    {
        onExitComplete?.Invoke();
    }
}
