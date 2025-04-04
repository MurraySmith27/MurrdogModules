using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiltingRoundUpdatePhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context, Action onEnterComplete)
    {
        PersistentState.Instance.IncrementRoundNumber();
        onEnterComplete?.Invoke();
    }
}
