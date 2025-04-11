using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomingResourceConversionController : Singleton<BloomingResourceConversionController>
{
    public event Action<int> OnStoneCollected;

    public event Action<int> OnWoodCollected;

    public event Action<ResourceType, long> OnResourceConvertedToBaseFoodScore;
    
    public event Action<ResourceType, long> OnResourceConvertedToFinalFoodScore;
    
    public void DoResourceConversion()
    {
        long foodScore = 0;

        Dictionary<ResourceType, int> currentResources = PlayerResourcesSystem.Instance.GetCurrentRoundResources();
        
        foreach (ResourceType key in currentResources.Keys)
        {
            if (key == ResourceType.Wood)
            {
                PlayerResourcesSystem.Instance.AddResource(PersistentResourceType.Wood, currentResources[key]);
                OnWoodCollected?.Invoke(currentResources[key]);
            }
            else if (key == ResourceType.Stone)
            {
                PlayerResourcesSystem.Instance.AddResource(PersistentResourceType.Stone, currentResources[key]);
                OnStoneCollected?.Invoke(currentResources[key]);
            }
            else
            {
                long foodScoreForResource = currentResources[key] * GameConstants.BASE_FOOD_SCORE_PER_RESOURCE[key];
                
                OnResourceConvertedToBaseFoodScore?.Invoke(key, foodScoreForResource);

                long foodScoreDifference = RelicSystem.Instance.OnConvertResourceToFoodScore(foodScoreForResource, key, currentResources[key]);

                foodScore += foodScoreForResource + foodScoreDifference;
                
                OnResourceConvertedToFinalFoodScore?.Invoke(key, foodScoreForResource + foodScoreDifference);
            }
        }
        
        HarvestState.Instance.AddHarvestFoodScore(foodScore);
        
        //clear existing resources
        PlayerResourcesSystem.Instance.RegisterCurrentRoundResources(new Dictionary<ResourceType, int>());
        
        RelicSystem.Instance.OnFoodScoreConversionComplete(foodScore, currentResources);
    }
}
