using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingHarvestPhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context)
    {
        BloomingHarvestController.Instance.StartHarvest();
    }
    
    public override void StateUpdate(PhaseStateMachine context)
    {
        
    }

    public override void StateExit(PhaseStateMachine context)
    {
        
    }
}
