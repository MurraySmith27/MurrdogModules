using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiltingChallengePhaseController : Singleton<WiltingChallengePhaseController>
{
    public event Action OnWiltingChallengePassed;
    
    public event Action OnWiltingChallengeFailed;
    
    public void StartChallengePhase()
    {
        long currentFoodGoal = RoundState.Instance.CurrentFoodGoal;

        if (RoundState.Instance.CurrentFoodScore >= RoundState.Instance.CurrentFoodGoal)
        {
            OnWiltingChallengePassed?.Invoke();
        }
        else
        {
            OnWiltingChallengeFailed?.Invoke();
        }
        
        RoundState.Instance.SetCurrentFoodScore(RoundState.Instance.CurrentFoodScore - RoundState.Instance.CurrentFoodGoal);
    }
}
