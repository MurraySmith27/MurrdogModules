using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddingBuildingPhase : PhaseStateBase
{

    public override void StateEnter(PhaseStateMachine context, Action onPhaseEnterComplete)
    {
        PersistentState.Instance.ChangeCurrentBuildTokens(1);
        onPhaseEnterComplete?.Invoke();
    }
}
