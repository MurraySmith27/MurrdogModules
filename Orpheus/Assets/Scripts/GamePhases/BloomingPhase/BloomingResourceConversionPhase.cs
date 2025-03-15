using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingResourceConversionPhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context)
    {
        Dictionary<ResourceType, int> resourcesToConvert = PlayerResourcesSystem.Instance.GetCurrentRoundResources();
        
        long foodScore = 0;
        
        //TODO
        
        RelicSystem.Instance.OnFoodScoreConversion(foodScore, resourcesToConvert);
    }
    
    public override void StateUpdate(PhaseStateMachine context)
    {
        
    }

    public override void StateExit(PhaseStateMachine context)
    {
        
    }
}
