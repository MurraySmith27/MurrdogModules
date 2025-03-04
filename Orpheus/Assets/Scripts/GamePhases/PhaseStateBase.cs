using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseStateBase : MonoBehaviour
{
    public virtual void StateEnter(PhaseStateMachine context)
    {
        
    }
    
    public virtual void StateUpdate(PhaseStateMachine context)
    {
        
    }

    public virtual void StateExit(PhaseStateMachine context)
    {
        
    }
}
