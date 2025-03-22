using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddingUpkeepPhase : PhaseStateBase
{
    public event Action<long> OnInterestApplied;
    
    public override void StateEnter(PhaseStateMachine context, Action onPhaseEnterComplete)
    {
     
        //firstly, apply interest
        long interestAcquired = (int)Math.Floor((Mathf.Min(RoundState.Instance.CurrentGold, GameConstants.GOLD_INTEREST_CAP) % GameConstants.GOLD_INTEREST_INTERVAL) * GameConstants.GOLD_INTEREST_PER_INTERVAL); 
        
        interestAcquired = RelicSystem.Instance.OnGoldInterestAdded(RoundState.Instance.CurrentGold, interestAcquired);
        
        PlayerResourcesSystem.Instance.AddResource(PersistentResourceType.Gold, interestAcquired);

        OnInterestApplied?.Invoke(interestAcquired);
        
        onPhaseEnterComplete?.Invoke();
    }
}
