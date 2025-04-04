using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestExtraResourceConversionController : Singleton<HarvestExtraResourceConversionController>
{
    public event Action<long, long> OnExtraResourcesConvertedToGold;
    
    public void ConvertRemainingFoodScoreToGold()
    {
        long extraFood = HarvestState.Instance.CurrentFoodScore;

        long goldAcquired = extraFood * GameConstants.GOLD_PER_LEFTOVER_FOOD_SCORE;
        PlayerResourcesSystem.Instance.AddResource(PersistentResourceType.Gold, goldAcquired);

        HarvestState.Instance.ResetFoodScore();
        
        OnExtraResourcesConvertedToGold?.Invoke(extraFood, goldAcquired);
    }
}
