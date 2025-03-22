using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiltingChallengePhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context, Action onPhaseEnterComplete)
    {
        WiltingChallengePhaseController.Instance.StartChallengePhase();
        
        onPhaseEnterComplete?.Invoke();
    }
}
