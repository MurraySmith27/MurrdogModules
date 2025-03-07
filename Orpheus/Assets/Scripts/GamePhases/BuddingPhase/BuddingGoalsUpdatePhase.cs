using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddingGoalsUpdatePhase : PhaseStateBase
{
    public override void StateEnter(PhaseStateMachine context)
    {
        RoundState.Instance.IncrementRoundNumber();
        IncreaseFoodGoal();
    }
    
    public override void StateUpdate(PhaseStateMachine context)
    {
        
    }

    public override void StateExit(PhaseStateMachine context)
    {
        
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
