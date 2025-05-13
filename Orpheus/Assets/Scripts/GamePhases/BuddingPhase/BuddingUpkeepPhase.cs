using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddingUpkeepPhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context, Action onEnterComplete)
    {
        //apply tech experience
        TechSystem.Instance.AddExp((int)HarvestState.Instance.CurrentFoodScore);
        
        onEnterComplete?.Invoke();
    }
}
