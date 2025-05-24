using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingEndStepPhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context, Action onEnterComplete)
    {
        // BloomingEndStepController.Instance.ApplyRoundBonusGold();
        
        onEnterComplete?.Invoke();
    }
}
