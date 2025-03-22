using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiltingExtraResourceConversionPhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context, Action onPhaseComplete)
    {
        WiltingExtraResourceConversionController.Instance.ConvertRemainingFoodScoreToGold();
        
        onPhaseComplete?.Invoke();
    }
}
