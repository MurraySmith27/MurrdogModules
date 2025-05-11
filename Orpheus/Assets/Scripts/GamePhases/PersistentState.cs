using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//stores all resource data that persists between rounds
public class PersistentState : Singleton<PersistentState>
{
    public int RoundNumber { get; private set; }
    
    public int HarvestNumber { get; private set; }
    
    public long CurrentWater { get; private set; }
    
    public long CurrentOil { get; private set; }
    
    public long CurrentDirt { get; private set; }

    public long CurrentGold { get; private set; } = GameConstants.STARTING_GOLD;

    public long CurrentWood { get; private set; } = GameConstants.STARTING_WOOD;
    
    public long CurrentStone { get; private set; } = GameConstants.STARTING_STONE;

    public long CurrentBuildTokens { get; private set; } = GameConstants.STARTING_BUILD_TOKENS;
    
    public int CurrentItemCapacity { get; private set; } = GameConstants.STARTING_ITEM_CAPACITY;

    public event Action<int> OnRoundEnd;
    
    public event Action<long> OnGoldValueChanged;

    public event Action<long> OnWaterValueChanged;
    
    public event Action<long> OnDirtValueChanged;
    
    public event Action<long> OnOilValueChanged;
    
    public event Action<long> OnWoodValueChanged;
    
    public event Action<long> OnStoneValueChanged;

    public event Action<long> OnBuildTokensValueChanged;
    
    public event Action<int> OnItemCapacityChanged;
    
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
        CurrentGold += difference;

        OnGoldValueChanged?.Invoke(CurrentGold);
    }

    public void ChangeCurrentWater(long difference)
    {
        CurrentWater += difference;

        OnWaterValueChanged?.Invoke(CurrentWater);
    }
    
    public void ChangeCurrentDirt(long difference)
    {
        CurrentDirt += difference;

        OnDirtValueChanged?.Invoke(CurrentDirt);
    }
    
    public void ChangeCurrentOil(long difference)
    {
        CurrentOil += difference;

        OnOilValueChanged?.Invoke(CurrentOil);
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
    
    public void ChangeCurrentBuildTokens(long difference)
    {
        CurrentBuildTokens += difference;
        
        OnBuildTokensValueChanged?.Invoke(CurrentBuildTokens);
    }

    public void ChangeItemCapacity(int newCapacity)
    {
        CurrentItemCapacity = newCapacity;

        OnItemCapacityChanged?.Invoke(newCapacity);
    }
}
