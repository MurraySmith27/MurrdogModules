using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//stores all resource data that persists between rounds
public class RoundState : Singleton<RoundState>
{
    public int RoundNumber { get; private set; }
    
    public long CurrentFoodGoal { get; private set; }
    
    public long CurrentFoodScore { get; private set; }

    public long CurrentGold { get; private set; } = GameConstants.STARTING_GOLD;

    public long CurrentWood { get; private set; } = GameConstants.STARTING_WOOD;
    
    public long CurrentStone { get; private set; } = GameConstants.STARTING_STONE;

    public event Action<int> OnRoundEnd;

    public event Action<long> OnFoodGoalChanged;
    
    public event Action<long> OnGoldValueChanged;
    
    public event Action<long> OnWoodValueChanged;
    
    public event Action<long> OnStoneValueChanged;

    public event Action<long> OnCurrentFoodScoreChanged;
    
    public void IncrementRoundNumber()
    {
        RoundNumber++;
        OnRoundEnd?.Invoke(RoundNumber);
    }

    public void ChangeCurrentFoodGoal(long newFoodGoal)
    {
        CurrentFoodGoal = newFoodGoal;
        
        OnFoodGoalChanged?.Invoke(newFoodGoal);
    }

    public void ChangeCurrentGold(long difference)
    {
        CurrentGold += difference;

        OnGoldValueChanged?.Invoke(CurrentGold);
    }

    public void SetCurrentFoodScore(long foodScore)
    {
        CurrentFoodScore = foodScore;

        OnCurrentFoodScoreChanged?.Invoke(foodScore);
    }
    
    public void ChangeCurrentWood(long difference)
    {
        CurrentWood += difference;

        OnWoodValueChanged?.Invoke(CurrentWood);
    }
    
    public void ChangeCurrentStone(long difference)
    {
        CurrentStone += difference;

        OnStoneValueChanged?.Invoke(CurrentStone);
    }
}
