using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//stores all resource data that persists between rounds
public class PersistentState : Singleton<PersistentState>
{
    public int RoundNumber { get; private set; }
    
    public int HarvestNumber { get; private set; }
    
    public int CurrentBonusCitizens { get; private set; }

    public long CurrentGold { get; private set; } = GameConstants.STARTING_GOLD;

    public long CurrentWood { get; private set; } = GameConstants.STARTING_WOOD;
    
    public long CurrentStone { get; private set; } = GameConstants.STARTING_STONE;

    public event Action<int> OnRoundEnd;
    
    public event Action<int> OnBonusCitizensValueChanged;
    
    public event Action<long> OnGoldValueChanged;
    
    public event Action<long> OnWoodValueChanged;
    
    public event Action<long> OnStoneValueChanged;
    
    public void IncrementRoundNumber()
    {
        RoundNumber++;
        OnRoundEnd?.Invoke(RoundNumber);
    }

    public void IncrementHarvestNumber()
    {
        HarvestNumber++;
    }
    
    public void ChangeCurrentBonusCitizens(int difference)
    {
        CurrentBonusCitizens += difference;
        OnBonusCitizensValueChanged?.Invoke(CurrentBonusCitizens);
    }
    
    public void ChangeCurrentGold(long difference)
    {
        CurrentGold += difference;

        OnGoldValueChanged?.Invoke(CurrentGold);
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
