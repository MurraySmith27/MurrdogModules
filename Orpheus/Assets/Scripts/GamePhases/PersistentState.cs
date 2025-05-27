using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//stores all resource data that persists between rounds
public class PersistentState : Singleton<PersistentState>
{
    public int RoundNumber { get; private set; }
    
    public int HarvestNumber { get; private set; }

    public long CurrentWater { get; private set; } = 1000;

    public long CurrentOil { get; private set; } = 1000;

    public long CurrentDirt { get; private set; } = 1000;

    public long CurrentGold { get; private set; } = GameConstants.STARTING_GOLD;

    public long CurrentWood { get; private set; } = GameConstants.STARTING_WOOD;
    
    public long CurrentStone { get; private set; } = GameConstants.STARTING_STONE;

    public long CurrentBuildTokens { get; private set; } = GameConstants.STARTING_BUILD_TOKENS;
    
    public int CurrentItemCapacity { get; private set; } = GameConstants.STARTING_ITEM_CAPACITY;

    public int CurrentCitizenCount { get; private set; } = GameConstants.STARTING_CITIZENS_PER_HARVEST_ROUND;
    
    
    public int CurrentHandsPerRound { get; private set; } = GameConstants.STARTING_HANDS_PER_HARVEST_ROUND;
    
    
    public int CurrentDiscardsPerRound { get; private set; } = GameConstants.STARTING_DISCARDS_PER_HARVEST_ROUND;

    public event Action<int> OnRoundEnd;
    
    public event Action<long> OnGoldValueChanged;

    public event Action<long> OnWaterValueChanged;
    
    public event Action<long> OnDirtValueChanged;
    
    public event Action<long> OnOilValueChanged;
    
    public event Action<long> OnWoodValueChanged;
    
    public event Action<long> OnStoneValueChanged;

    public event Action<long> OnBuildTokensValueChanged;

    public event Action<int> OnCitizenCountChanged;

    public event Action<int> OnHandsPerRoundChanged;
    
    public event Action<int> OnDiscardsPerRoundChanged;
    
    public event Action<int> OnItemCapacityChanged;

    private void Start()
    {
        TechSystem.Instance.OnLevelUp -= OnLevelUp;
        TechSystem.Instance.OnLevelUp += OnLevelUp;
    }

    private void OnDestroy()
    {
        if (TechSystem.IsAvailable)
        {
            TechSystem.Instance.OnLevelUp -= OnLevelUp;
        }
    }

    private void OnLevelUp(int level)
    {
        ChangeCitizensPerRound(TechSystem.Instance.GetRewardOfLevel(level, TechSystemRewardType.CITIZENS));
        ChangeHandsPerRound(TechSystem.Instance.GetRewardOfLevel(level, TechSystemRewardType.HANDS));
        ChangeDiscardsPerRound(TechSystem.Instance.GetRewardOfLevel(level, TechSystemRewardType.DISCARDS));
    }
    
    public void IncrementRoundNumber()
    {
        RoundNumber++;
        OnRoundEnd?.Invoke(RoundNumber);
    }

    public void IncrementHarvestNumber()
    {
        HarvestNumber++;
    }
    
    public void ChangeCurrentGold(long difference)
    {
        if (difference == 0) return;
        CurrentGold += difference;

        OnGoldValueChanged?.Invoke(CurrentGold);
    }

    public void ChangeCurrentWater(long difference)
    {
        if (difference == 0) return;
        CurrentWater += difference;

        OnWaterValueChanged?.Invoke(CurrentWater);
    }
    
    public void ChangeCurrentDirt(long difference)
    {
        if (difference == 0) return;
        CurrentDirt += difference;

        OnDirtValueChanged?.Invoke(CurrentDirt);
    }
    
    public void ChangeCurrentOil(long difference)
    {
        if (difference == 0) return;
        CurrentOil += difference;

        OnOilValueChanged?.Invoke(CurrentOil);
    }
    
    public void ChangeCurrentWood(long difference)
    {
        if (difference == 0) return;
        CurrentWood += difference;

        OnWoodValueChanged?.Invoke(CurrentWood);
    }
    
    public void ChangeCurrentStone(long difference)
    {
        if (difference == 0) return;
        CurrentStone += difference;

        OnStoneValueChanged?.Invoke(CurrentStone);
    }
    
    public void ChangeCurrentBuildTokens(long difference)
    {
        if (difference == 0) return;
        CurrentBuildTokens += difference;
        
        OnBuildTokensValueChanged?.Invoke(CurrentBuildTokens);
    }

    public void ChangeCitizensPerRound(int difference)
    {
        if (difference == 0) return;
        CurrentCitizenCount += difference;
        
        OnCitizenCountChanged?.Invoke(CurrentCitizenCount);
    }
    
    public void ChangeHandsPerRound(int difference)
    {
        if (difference == 0) return;
        CurrentHandsPerRound += difference;
        
        OnHandsPerRoundChanged?.Invoke(CurrentHandsPerRound);
    }
    
    public void ChangeDiscardsPerRound(int difference)
    {
        if (difference == 0) return;
        CurrentDiscardsPerRound += difference;
        
        OnDiscardsPerRoundChanged?.Invoke(CurrentDiscardsPerRound);
    }

    public void ChangeItemCapacity(int newCapacity)
    {
        CurrentItemCapacity = newCapacity;

        OnItemCapacityChanged?.Invoke(newCapacity);
    }
}
