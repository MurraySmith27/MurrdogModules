using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddingGoalsUpdatePhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context, Action onPhaseEnterComplete)
    {
        RoundState.Instance.IncrementRoundNumber();
        IncreaseFoodGoal();

        onPhaseEnterComplete?.Invoke();
    }
    
    private void IncreaseFoodGoal()
    {
        //use a 
        int currentRound = RoundState.Instance.RoundNumber;

        if (currentRound < GameConstants.FOOD_GOALS_PER_ROUND.Length)
        {
            RoundState.Instance.ChangeCurrentFoodGoal(GameConstants.FOOD_GOALS_PER_ROUND[currentRound]);
        }
    }
}
