using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingResourceConversionPhase : PhaseStateBase
{
    public virtual void StateEnter(PhaseStateMachine context, Action onEnterComplete)
    {
        onEnterComplete?.Invoke();
    }
}
