using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingUpkeepPhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context, Action onEnterComplete)
    {
        PersistentState.Instance.IncrementHarvestNumber();

        HarvestState.Instance.SetFoodGoalForHarvest(PersistentState.Instance.HarvestNumber);
        
        HarvestState.Instance.ResetFoodScore();
        
        onEnterComplete?.Invoke();
    }
}
