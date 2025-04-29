using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestState : Singleton<HarvestState>
{
    public int NumRemainingCitizens
    {
        get;
        private set;
    }

    public int NumRemainingDiscards
    {
        get;
        private set;
    }

    public int NumRemainingHands
    {
        get;
        private set;
    }

    public int NumCitizensUsedThisHarvest
    {
        get;
        private set;
    }

    public int NumBonusCitizensUsedThisHarvest
    {
        get;
        private set;
    }
    
    public int NumDiscardsUsed
    {
        get;
        private set;
    }

    public int NumHandsUsed
    {
        get;
        private set;
    }

    public event Action OnFoodGoalReached;
    
    public long CurrentFoodGoal { get; private set; }
    
    public event Action<long> OnFoodGoalChanged;
    
    public long CurrentFoodScore { get; private set; }
    
    public event Action<long> OnCurrentFoodScoreChanged;

    public event Action OnHarvestFailed;

    public event Action OnHarvestStart;
    
    private void Start()
    {
        PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnPhaseChanged;
    }
    
    private void OnDestroy()
    {
        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        }    
    }

    private void OnPhaseChanged(GamePhases phase)
    {
        if (phase == GamePhases.BloomingUpkeep)
        {
            NumRemainingCitizens = GameConstants.STARTING_CITIZENS_PER_HARVEST_ROUND;
            NumCitizensUsedThisHarvest = 0;
            NumRemainingDiscards = GameConstants.STARTING_DISCARDS_PER_HARVEST_ROUND;
            NumDiscardsUsed = 0;
            NumRemainingHands = GameConstants.STARTING_HANDS_PER_HARVEST_ROUND;
            NumHandsUsed = 0;

            NumBonusCitizensUsedThisHarvest = 0;

            RelicSystem.Instance.OnHarvestStart();
            
            OnHarvestStart?.Invoke();
        }
    }
    
    private void ChangeCurrentFoodGoal(long newFoodGoal)
    {
        CurrentFoodGoal = newFoodGoal;
        
        OnFoodGoalChanged?.Invoke(newFoodGoal);
    }

    public void AddHarvestFoodScore(long foodScore)
    {
        if (foodScore != 0)
        {
            CurrentFoodScore += foodScore;
            OnCurrentFoodScoreChanged?.Invoke(CurrentFoodScore);
        }
    }

    public void CheckForWinOrLose()
    {
        if (CurrentFoodScore >= CurrentFoodGoal)
        {
            OnFoodGoalReached?.Invoke();
        }
        else
        {
            if (NumRemainingHands == 0)
            {
                OnHarvestFailed?.Invoke();
            }
        }
    }

    public void ResetFoodScore()
    {
        CurrentFoodScore = 0;
    }
    
    public void UseCitizens(int numCitizensUsed)
    {
        NumCitizensUsedThisHarvest += numCitizensUsed;
    }

    public void UseBonusCitizens(int numBonusCitizensUsed)
    {
        UseCitizens(numBonusCitizensUsed);

        NumBonusCitizensUsedThisHarvest += numBonusCitizensUsed;
    }
    
    public void UseDiscard()
    {
        NumRemainingDiscards--;
        NumDiscardsUsed++;
    }

    public void UseHand()
    {
        NumRemainingHands--;
        NumHandsUsed++;
    }

    public void AddExtraHands(int numExtraHands)
    {
        NumRemainingHands += numExtraHands;
    }
    
    public void AddExtraDiscards(int numExtraDiscards)
    {
        NumRemainingDiscards += numExtraDiscards;
    }

    public void SetFoodGoalForHarvest(int currentHarvest)
    {
        if (currentHarvest < GameConstants.FOOD_GOALS_PER_HARVEST.Length)
        {
            ChangeCurrentFoodGoal(GameConstants.FOOD_GOALS_PER_HARVEST[currentHarvest]);
        }
        else
        {
            Debug.LogError($"tried to get food goal for harvest number {currentHarvest}, " +
                           $"but there are only {GameConstants.FOOD_GOALS_PER_HARVEST.Length} food goal defined.");
            return;
        }
    }
}
