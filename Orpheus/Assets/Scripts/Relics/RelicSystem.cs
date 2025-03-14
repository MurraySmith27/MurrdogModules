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
    STONE_ROSE
}

public struct AdditionalRelicTriggeredArgs
{
    public int IntArg;
    public Vector2 Vector2Arg;
    public string StringArg;
    public bool BoolArg;
    public List<Vector2> Vector2ListArgs;
    public List<Vector2Int> Vector2IntListArgs;
}

public class RelicSystem : Singleton<RelicSystem>
{
    private Dictionary<RelicTypes, Relic> _relicInstances = new Dictionary<RelicTypes, Relic>();
    
    private List<RelicTypes> relics = new List<RelicTypes>();
    
    public event Action<RelicTypes> OnRelicAdded;
    public event Action<RelicTypes> OnRelicRemoved;

    public event Action<RelicTypes, AdditionalRelicTriggeredArgs> OnRelicTriggered;
    
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
    
    public Dictionary<ResourceType, int> OnResourcesProcessed(Dictionary<ResourceType, int> resourceState)
    {
        //make a copy
        Dictionary<ResourceType, int> modifiedResources = resourceState.ToDictionary(entry => entry.Key, entry => entry.Value);
        
        foreach (RelicTypes relic in relics)
        {
            AdditionalRelicTriggeredArgs args;
            if (_relicInstances[relic].OnResourcesProcessed(modifiedResources, out modifiedResources, out args))
            {
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
        
        return modifiedResources;
    }
    
    public Dictionary<ResourceType, int> OnResourcesHarvested(Dictionary<ResourceType, int> resourcesToBeHarvested)
    {
        //make a copy
        Dictionary<ResourceType, int> modifiedResources = resourcesToBeHarvested.ToDictionary(entry => entry.Key, entry => entry.Value);
        
        foreach (RelicTypes relic in relics)
        {
            AdditionalRelicTriggeredArgs args;
            if (_relicInstances[relic].OnResourcesHarvested(modifiedResources, out modifiedResources, out args))
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
            AdditionalRelicTriggeredArgs args;
            if (_relicInstances[relic].OnPhaseChanged(phase, out args))
            {
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
    }
    
    public int OnGoldInterestAdded(int coinTotalBefore, int interest)
    {
        int currentInterest = interest;
        foreach (RelicTypes relic in relics)
        {
            AdditionalRelicTriggeredArgs args;
            if (_relicInstances[relic].OnGoldInterestAdded(coinTotalBefore, currentInterest, out currentInterest, out args))
            {
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
        
        return currentInterest;
    }
    
    public int OnFoodScoreConversion(int baseFoodScore, Dictionary<ResourceType, int> resourcesToConvert)
    {
        int currentFoodScore = baseFoodScore;
        
        foreach (RelicTypes relic in relics)
        {
            AdditionalRelicTriggeredArgs args;
            if (_relicInstances[relic].OnFoodScoreConversion(currentFoodScore, resourcesToConvert, out currentFoodScore, out args))
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
            AdditionalRelicTriggeredArgs args;
            if (_relicInstances[relic].OnBuildingConstructed(position, buildingType, out args))
            {
                OnRelicTriggered?.Invoke(relic, args);
            }
        }
    }
}
