using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingUpkeepPhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context, Action onEnterComplete)
    {
        HarvestState.Instance.SetFoodGoalForHarvest(PersistentState.Instance.HarvestNumber);
        
        PersistentState.Instance.IncrementHarvestNumber();
        
        HarvestState.Instance.ResetFoodScore();
        
        onEnterComplete?.Invoke();
    }
}
