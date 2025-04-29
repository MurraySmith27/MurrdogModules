using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingEndStepController : Singleton<BloomingEndStepController>
{
    public event Action<long> OnHarvestGoldApplied;
    public event Action<long> OnInterestApplied;
    
    public long LastInterestRecieved
    {
        get;
        private set;
    }
    
    public long LastHarvestGoldReiceived
    {
        get;
        private set;
    }

    public void ApplyRoundBonusGold()
    {
        ApplyHarvestGold();
        ApplyInterest();
    }

    private void ApplyHarvestGold()
    {
        LastHarvestGoldReiceived = HarvestState.Instance.NumRemainingHands * GameConstants.HARVEST_GOLD_PER_UNUSED_HAND +
                                   GameConstants.GOLD_PER_HARVEST;
        PersistentState.Instance.ChangeCurrentGold(LastHarvestGoldReiceived);
        
        OnHarvestGoldApplied?.Invoke(LastHarvestGoldReiceived);
    }

    private void ApplyInterest()
    {
        //firstly, apply interest
        long interestAcquired = (int)Math.Floor((Mathf.Min(PersistentState.Instance.CurrentGold, GameConstants.GOLD_INTEREST_CAP) / GameConstants.GOLD_INTEREST_INTERVAL) * GameConstants.GOLD_INTEREST_PER_INTERVAL); 
        
        interestAcquired = RelicSystem.Instance.OnGoldInterestAdded(PersistentState.Instance.CurrentGold, interestAcquired);
        
        PersistentState.Instance.ChangeCurrentGold(interestAcquired);
        
        LastInterestRecieved = interestAcquired;

        OnInterestApplied?.Invoke(interestAcquired);
    }
}
