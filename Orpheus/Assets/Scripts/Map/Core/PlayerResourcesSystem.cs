using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for tracking the player's owned resources
public class PlayerResourcesSystem : Singleton<PlayerResourcesSystem>
{
    private Dictionary<ResourceType, int> _currentRoundResources = new();

    public event Action<Dictionary<ResourceType, int>> OnCurrentRoundResourcesChange;

    public event Action<Vector2Int, PersistentResourceType, int> OnBaseResourcesGranted;
    
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
            case PersistentResourceType.Water:
                PersistentState.Instance.ChangeCurrentWater(diff);
                break;
            case PersistentResourceType.Dirt:
                PersistentState.Instance.ChangeCurrentDirt(diff);
                break;
            case PersistentResourceType.Oil:
                PersistentState.Instance.ChangeCurrentOil(diff);
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
            case PersistentResourceType.Water:
                return PersistentState.Instance.CurrentWater >= quantity;
            case PersistentResourceType.Dirt:
                return PersistentState.Instance.CurrentDirt >= quantity;
            case PersistentResourceType.Oil:
                return PersistentState.Instance.CurrentOil >= quantity;
            
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

    public void AddTileTypeResources()
    {
        List<Vector2Int> cityPositions = MapSystem.Instance.GetAllOwnedCityTiles();

        int numOil = 0, numDirt = 0, numWater = 0;
        
        foreach (Vector2Int cityPosition in cityPositions)
        {
            TileType type = MapSystem.Instance.GetTileType(cityPosition.x, cityPosition.y);

            switch (type)
            {
                case TileType.Grass:
                    OnBaseResourcesGranted?.Invoke(cityPosition, PersistentResourceType.Dirt, 1);
                    numDirt++;
                    break;
                case TileType.Water:
                    OnBaseResourcesGranted?.Invoke(cityPosition, PersistentResourceType.Water, 1);
                    numWater++;
                    break;
                case TileType.Desert:
                    OnBaseResourcesGranted?.Invoke(cityPosition, PersistentResourceType.Oil, 1);
                    numOil++;
                    break;
                default:
                    break;
            }
        }
        
        AddResource(PersistentResourceType.Dirt, numDirt);
        AddResource(PersistentResourceType.Water, numWater);
        AddResource(PersistentResourceType.Oil, numOil);
    }
}
