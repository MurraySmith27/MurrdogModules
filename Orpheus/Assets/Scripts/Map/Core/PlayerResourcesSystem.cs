using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for tracking the player's owned resources
public class PlayerResourcesSystem : Singleton<PlayerResourcesSystem>
{
    private Dictionary<ResourceType, int> _currentRoundResources = new();

    public event Action<Dictionary<ResourceType, int>> OnCurrentRoundResourcesChange;
    
    public void AddResource(PersistentResourceType resourceType, long quantity)
    {
        ModifyResource(resourceType, quantity);
    }

    public void SpendResource(PersistentResourceType resourceType, long quantity)
    {
        ModifyResource(resourceType, -quantity);
    }

    private void ModifyResource(PersistentResourceType resourceType, long diff)
    {
        switch (resourceType)
        {
            case PersistentResourceType.Wood:
                PersistentState.Instance.ChangeCurrentWood(diff);
                break;
            case PersistentResourceType.Stone:
                PersistentState.Instance.ChangeCurrentStone(diff);
                break;
            case PersistentResourceType.Gold:
                PersistentState.Instance.ChangeCurrentGold(diff);
                break;
            case PersistentResourceType.BuildToken:
                PersistentState.Instance.ChangeCurrentBuildTokens(diff);
                break;
        }
    }

    public bool HasResource(PersistentResourceType resourceType, long quantity)
    {
        switch (resourceType)
        {
            case PersistentResourceType.Wood:
                return PersistentState.Instance.CurrentWood >= quantity;
            case PersistentResourceType.Stone:
                return PersistentState.Instance.CurrentStone >= quantity;
            case PersistentResourceType.Gold:
                return PersistentState.Instance.CurrentGold >= quantity;
            case PersistentResourceType.BuildToken:
                return PersistentState.Instance.CurrentBuildTokens >= quantity;
        }

        return false;
    }

    public bool PayCost(List<PersistentResourceItem> costs)
    {
        foreach (PersistentResourceItem cost in costs)
        {
            if (!HasResource(cost.Type, cost.Quantity)) return false;
        }
        
        foreach (PersistentResourceItem cost in costs)
        {
            ModifyResource(cost.Type, -cost.Quantity);
        }

        return true;
    }

    public void RegisterCurrentRoundResources(Dictionary<ResourceType, int> currentRoundResources)
    {
        _currentRoundResources = currentRoundResources;
        
        OnCurrentRoundResourcesChange?.Invoke(currentRoundResources);
    }

    public Dictionary<ResourceType, int> GetCurrentRoundResources()
    {
        return _currentRoundResources;
    }
}
