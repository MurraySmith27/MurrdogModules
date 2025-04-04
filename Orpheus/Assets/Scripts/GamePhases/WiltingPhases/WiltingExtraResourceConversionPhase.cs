using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiltingExtraResourceConversionPhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context, Action onPhaseComplete)
    {
        HarvestExtraResourceConversionController.Instance.ConvertRemainingFoodScoreToGold();
        
        onPhaseComplete?.Invoke();
    }
}
