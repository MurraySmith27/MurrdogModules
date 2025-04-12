using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum RelicTypes
{
    NONE,
    PRIVATE_EYES,
    RUSTY_PLOWSHARE,
    CAPTAINS_HOOK,
    BAKERS_DOZEN,
    LUCKY_COIN,
    BUSINESS_CARD,
    JELLY_DONUT,
    TATTERED_MAP,
    THE_MOLLUSK,
    STONE_ROSE,
    EXTRA_HAND,
    EXTRA_DISCARD
}

public struct AdditionalTriggeredArgs
{
    public int IntArg;
    public long LongArg;
    public Vector2 Vector2Arg;
    public string StringArg;
    public bool BoolArg;
    public List<int> IntListArgs;
    public List<long> LongListArgs;
    public List<Vector2> Vector2ListArgs;
    public List<Vector2Int> Vector2IntListArgs;
    public List<Guid> GuidListArgs;
}

public class RelicSystem : Singleton<RelicSystem>
{
    private Dictionary<RelicTypes, Relic> _relicInstances = new Dictionary<RelicTypes, Relic>();
    
    private List<RelicTypes> relics = new List<RelicTypes>();
    
    public event Action<RelicTypes> OnRelicAdded;
    public event Action<RelicTypes> OnRelicRemoved;
    public event Action<RelicTypes, AdditionalTriggeredArgs> OnRelicTriggered;
    
    private void Awake()
    {
        RelicFactory relicFactory = new RelicFactory();
        
        for (int i = 0; i < Enum.GetValues(typeof(RelicTypes)).Length; i++)
        {
            _relicInstances.Add((RelicTypes)i, relicFactory.CreateRelic((RelicTypes)i));
        }
    }
    
    public void AddRelic(RelicTypes relic)
    {
        if (!relics.Contains(relic))
        {
            relics.Add(relic);
            OnRelicAdded?.Invoke(relic);
        }
    }

    public void RemoveRelic(RelicTypes relic)
    {
        if (relics.Contains(relic))
        {
            relics.Remove(relic);
            OnRelicRemoved?.Invoke(relic);
        }
    }
    
    public bool HasRelic(RelicTypes relic)
    {
        return relics.Contains(relic);
    }

    public List<RelicTypes> GetOwnedRelics()
    {
        return relics;
    }
    
    public Dictionary<ResourceType, int> OnResourcesProcessed(Dictionary<ResourceType, int> resourceDiff, Vector2Int position)
    {
        //make a copy
        Dictionary<ResourceType, int> modifiedResourceDiff = resourceDiff.ToDictionary(entry => entry.Key, entry => entry.Value);
        
        foreach (RelicTypes relic in relics)
        {
            AdditionalTriggeredArgs args;
            if (_relicInstances[relic].OnResourcesProcessed(modifiedResourceDiff, position, out modifiedResourceDiff, out args))
            {
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
        
        return modifiedResourceDiff;
    }
    
    public Dictionary<ResourceType, int> OnResourcesHarvested(Dictionary<ResourceType, int> resourcesToBeHarvested, Vector2Int position)
    {
        //make a copy
        Dictionary<ResourceType, int> modifiedResources = resourcesToBeHarvested.ToDictionary(entry => entry.Key, entry => entry.Value);
        
        foreach (RelicTypes relic in relics)
        {
            AdditionalTriggeredArgs args;
            if (_relicInstances[relic].OnResourcesHarvested(modifiedResources, position, out modifiedResources, out args))
            {
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
        
        return modifiedResources;
    }

    public void OnPhaseChanged(GamePhases phase)
    {
        foreach (RelicTypes relic in relics)
        {
            AdditionalTriggeredArgs args;
            if (_relicInstances[relic].OnPhaseChanged(phase, out args))
            {
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
    }
    
    public long OnGoldInterestAdded(long coinTotalBefore, long interest)
    {
        long currentInterest = interest;
        foreach (RelicTypes relic in relics)
        {
            AdditionalTriggeredArgs args;
            if (_relicInstances[relic].OnGoldInterestAdded(coinTotalBefore, currentInterest, out currentInterest, out args))
            {
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
        
        return currentInterest;
    }
    
    public List<(RelicTypes, long)> OnFoodHarvestedQuantityCalculated(long quantitySoFar, ResourceType resourceType, out long changeInQuantity)
    {
        List<(RelicTypes, long)> relicsTriggered = new();
        
        long currentQuantity = quantitySoFar;

        foreach (RelicTypes relic in relics)
        {
            AdditionalTriggeredArgs args;

            if (_relicInstances[relic].OnFoodHarvestedQuantityCalculated(currentQuantity, resourceType, out long quantityDifference, out args))
            {
                relicsTriggered.Add((relic, quantityDifference));
                
                currentQuantity += quantityDifference;
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
        
        changeInQuantity = currentQuantity - quantitySoFar;

        return relicsTriggered;
    }
    
    public List<(RelicTypes, double)> OnFoodHarvestedMultCalculated(double multSoFar, ResourceType resourceType, out double changeInMult)
    {
        List<(RelicTypes, double)> relicsTriggered = new();
        
        double currentMult = multSoFar;

        foreach (RelicTypes relic in relics)
        {
            AdditionalTriggeredArgs args;

            if (_relicInstances[relic].OnFoodHarvestedMultCalculated(currentMult, resourceType, out double multDifference, out args))
            {
                relicsTriggered.Add((relic, multDifference));
                
                currentMult += multDifference;
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
        
        changeInMult = currentMult - multSoFar;

        return relicsTriggered;
    }

    public List<(RelicTypes, long, double)> OnConvertResourceToFoodScore(ResourceType resourceType, long quantitySoFar, double multSoFar, out long quantityChange, out double multChange)
    {
        List<(RelicTypes, long, double)> relicsTriggered = new();
        
        long currentQuantity = quantitySoFar;
        double currentMult = multSoFar; 

        foreach (RelicTypes relic in relics)
        {
            AdditionalTriggeredArgs args;

            if (_relicInstances[relic].OnConvertResourceToFoodScore(resourceType, currentQuantity, currentMult, out long quantityDifference, out double multDifference, out args))
            {
                relicsTriggered.Add((relic, quantityDifference, multDifference));
                
                currentQuantity += quantityDifference;
                currentMult += multDifference;
                OnRelicTriggered?.Invoke(relic, args);
            }
        }

        quantityChange = currentQuantity - quantitySoFar;
        multChange = currentMult - multSoFar;

        return relicsTriggered;
    }
    
    public long OnFoodScoreConversionComplete(long baseFoodScore, Dictionary<ResourceType, int> resourcesToConvert)
    {
        long currentFoodScore = baseFoodScore;
        
        foreach (RelicTypes relic in relics)
        {
            AdditionalTriggeredArgs args;
            if (_relicInstances[relic].OnFoodScoreConversionComplete(currentFoodScore, resourcesToConvert, out currentFoodScore, out args))
            {
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
        
        return currentFoodScore;
    }
    
    public void OnBuildingConstructed(Vector2Int position, BuildingType buildingType)
    {
        foreach (RelicTypes relic in relics)
        {
            AdditionalTriggeredArgs args;
            if (_relicInstances[relic].OnBuildingConstructed(position, buildingType, out args))
            {
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
    }

    public void OnHarvestStart()
    {
        foreach (RelicTypes relic in relics)
        {
            AdditionalTriggeredArgs args;
            if (_relicInstances[relic].OnHarvestStart(out args))
            {
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
    }
}
