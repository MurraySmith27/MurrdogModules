using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;

public class BloomingResourceConversionController : Singleton<BloomingResourceConversionController>
{
    [SerializeField] private float startAnimationTime = 1f;
    [SerializeField] private float endAnimationTime = 1f;
    [SerializeField] private float resourceConversionStartTime = 0.6f;
    [SerializeField] private float resourceQuantityProcessTime = 0.3f;
    [SerializeField] private float resourceMultProcessTime = 0.3f;
    [SerializeField] private float resourceFoodScoreProcessTime = 0.3f;
    [SerializeField] private float resourceConversionEndTime = 0.25f;
    [SerializeField] private float resourceConversionResourceFoodScoreAddedTime = 1f;

    public event Action OnResourceConversionStart;
    public event Action OnResourceConversionEnd;
    public event Action OnResourceConversionEndFinal;
    public event Action<ResourceType> OnResourceConversionResourceStart;
    public event Action<ResourceType, long> OnResourceConversionQuantityProcessed;
    public event Action<ResourceType, double> OnResourceConversionMultProcessed;
    public event Action<ResourceType, long> OnResourceConversionFoodScoreProcessed;
    public event Action<ResourceType> OnResourceConversionResourceEnd;
    public event Action<ResourceType, long> OnResourceConversionFoodScoreAddedStart;
    public event Action<ResourceType, long> OnResourceConversionFoodScoreAddedEnd;

    public event Action<long> OnResourceConversionBonusFoodScoreAdded;
    
    
    public void DoResourceConversion()
    {
        Dictionary<ResourceType, int> currentResources = PlayerResourcesSystem.Instance.GetCurrentRoundResources();

        foreach (ResourceType key in currentResources.Keys)
        {
            if (key == ResourceType.Wood)
            {
                PlayerResourcesSystem.Instance.AddResource(PersistentResourceType.Wood, currentResources[key]);
            }
            else if (key == ResourceType.Stone)
            {
                PlayerResourcesSystem.Instance.AddResource(PersistentResourceType.Stone, currentResources[key]);
            }
        }

        Timing.RunCoroutineSingleton(ResourceConversionCoroutine(currentResources), this.gameObject, SingletonBehavior.Overwrite);
    }

    private IEnumerator<float> ResourceConversionCoroutine(Dictionary<ResourceType, int> resources)
    {
        List<(ResourceType, long)> resourceFoodScores = new();
        
        OnResourceConversionStart?.Invoke();

        yield return OrpheusTiming.WaitForSecondsGameTime(startAnimationTime);
        
        foreach (ResourceType key in resources.Keys)
        {
            if (resources[key] == 0)
            {
                continue;
            }
            
            OnResourceConversionResourceStart?.Invoke(key);

            yield return OrpheusTiming.WaitForSecondsGameTime(resourceConversionStartTime);

            long currentQuantity = resources[key];
            
            OnResourceConversionQuantityProcessed?.Invoke(key, currentQuantity);
            
            yield return OrpheusTiming.WaitForSecondsGameTime(resourceQuantityProcessTime);
            
            List<(RelicTypes, long)> quantityRelicTypesTriggered = RelicSystem.Instance.OnFoodHarvestedQuantityCalculated(currentQuantity, key, out long foodQuantityChange);
            
            foreach ((RelicTypes, long) pair in quantityRelicTypesTriggered)
            {
                currentQuantity += pair.Item2;
                
                //TODO: Trigger relic shake
                
                OnResourceConversionQuantityProcessed?.Invoke(key, currentQuantity);
                yield return OrpheusTiming.WaitForSecondsGameTime(resourceQuantityProcessTime);
            }

            double currentMult = GameConstants.BASE_FOOD_SCORE_PER_RESOURCE[key];
            
            OnResourceConversionMultProcessed?.Invoke(key, currentMult);

            yield return OrpheusTiming.WaitForSecondsGameTime(resourceMultProcessTime);
            
            List<(RelicTypes, double)> multRelicTypesTriggered = RelicSystem.Instance.OnFoodHarvestedMultCalculated(currentMult, key, out double foodMultChange);

            foreach ((RelicTypes, double) pair in multRelicTypesTriggered)
            {
                currentMult += pair.Item2;
                
                //TODO: Trigger relic shake
                
                OnResourceConversionMultProcessed?.Invoke(key, currentMult);
                yield return OrpheusTiming.WaitForSecondsGameTime(resourceMultProcessTime);
            }

            yield return OrpheusTiming.WaitForSecondsGameTime(resourceMultProcessTime);
            
            List<(RelicTypes, long, double)> foodScoreRelicTypesTriggered = RelicSystem.Instance.OnConvertResourceToFoodScore(key, currentQuantity, currentMult, out long quantityChange, out double multChange);

            foreach ((RelicTypes, long, double) pair in foodScoreRelicTypesTriggered)
            {
                currentQuantity += pair.Item2;
                currentMult += pair.Item3;
                
                //TODO: Trigger relic shake
                
                if (pair.Item2 != 0)
                {
                    OnResourceConversionQuantityProcessed?.Invoke(key, currentQuantity);
                    yield return OrpheusTiming.WaitForSecondsGameTime(resourceQuantityProcessTime);
                }

                if (pair.Item3 != 0)
                {
                    OnResourceConversionMultProcessed?.Invoke(key, currentMult);
                    yield return OrpheusTiming.WaitForSecondsGameTime(resourceMultProcessTime);
                }
            }

            long resourceFoodScore = (long)Math.Round(currentQuantity * currentMult);
            
            OnResourceConversionFoodScoreProcessed?.Invoke(key, resourceFoodScore);
            
            yield return OrpheusTiming.WaitForSecondsGameTime(resourceFoodScoreProcessTime);
            
            resourceFoodScores.Add((key, resourceFoodScore));
            
            OnResourceConversionResourceEnd?.Invoke(key);

            yield return OrpheusTiming.WaitForSecondsGameTime(resourceConversionEndTime);
        }

        long foodScoreTotal = 0;
        
        foreach ((ResourceType, long) pair in resourceFoodScores)
        {
            foodScoreTotal += pair.Item2;
            
            OnResourceConversionFoodScoreAddedStart?.Invoke(pair.Item1, pair.Item2);
            
            HarvestState.Instance.AddHarvestFoodScore(pair.Item2);

            yield return OrpheusTiming.WaitForSecondsGameTime(resourceConversionResourceFoodScoreAddedTime);
            
            OnResourceConversionFoodScoreAddedEnd?.Invoke(pair.Item1, pair.Item2);
        }
        
        //clear existing resources
        PlayerResourcesSystem.Instance.RegisterCurrentRoundResources(new Dictionary<ResourceType, int>());

        if (RelicSystem.Instance.OnFoodScoreConversionComplete(foodScoreTotal, resources, out long outFoodScore))
        {
            HarvestState.Instance.AddHarvestFoodScore(outFoodScore - foodScoreTotal);
            OnResourceConversionBonusFoodScoreAdded?.Invoke(outFoodScore);
            yield return OrpheusTiming.WaitForSecondsGameTime(resourceConversionResourceFoodScoreAddedTime);
        }

        
        OnResourceConversionEnd?.Invoke();
        
        HarvestState.Instance.CheckForWinOrLose();
        
        yield return OrpheusTiming.WaitForSecondsGameTime(endAnimationTime);
        
        OnResourceConversionEndFinal?.Invoke();

    }
}
