using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiltingExtraResourceConversionController : Singleton<WiltingExtraResourceConversionController>
{
    public event Action<long, long> OnExtraResourcesConvertedToGold;
    
    public void ConvertRemainingFoodScoreToGold()
    {
        long extraFood = RoundState.Instance.CurrentFoodScore;

        long goldAcquired = extraFood * GameConstants.GOLD_PER_LEFTOVER_FOOD_SCORE;
        PlayerResourcesSystem.Instance.AddResource(PersistentResourceType.Gold, goldAcquired);

        RoundState.Instance.SetCurrentFoodScore(0);
        
        OnExtraResourcesConvertedToGold?.Invoke(extraFood, goldAcquired);
    }
}
