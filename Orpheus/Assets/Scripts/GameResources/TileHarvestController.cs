using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TileHarvestController : Singleton<TileHarvestController>
{
    public Dictionary<ResourceType, int> GetResourceChangeOnTileHarvest(Guid cityGuid, Vector2Int tilePosition, Dictionary<ResourceType, int> resourcesSoFar)
    {
        //get resources on tile
        List<ResourceItem> resourcesOnTile = MapSystem.Instance.GetAllResourcesOnTile(tilePosition);
        
        Dictionary<ResourceType, int> resourcesHarvested = new Dictionary<ResourceType, int>();
        Dictionary<ResourceType, int> resourcesOnTileDictionary = new Dictionary<ResourceType, int>();
        
        //do harvest
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            resourcesHarvested[resourceType] = 0;
            resourcesOnTileDictionary[resourceType] = 0;
        }

        foreach (ResourceItem resourceItem in resourcesOnTile)
        {
            resourcesHarvested[resourceItem.Type] += resourceItem.Quantity;
            resourcesOnTileDictionary[resourceItem.Type] = resourceItem.Quantity;
        }
        
        List<TileBuilding> buildingsOnTile = MapSystem.Instance.GetBuildingsOnTile(tilePosition);

        foreach (TileBuilding building in buildingsOnTile)
        {
            switch (building.Type)
            {
                case BuildingType.CornFarm:
                    resourcesHarvested[ResourceType.Corn] += GameConstants.CORN_PER_CORN_FARM;
                    break;
                
                case BuildingType.WheatFarm:
                    resourcesHarvested[ResourceType.Wheat] += GameConstants.WHEAT_PER_WHEAT_FARM;
                    break;
                
                case BuildingType.FishFarm:
                    resourcesHarvested[ResourceType.Fish] += GameConstants.FISH_PER_FISH_FARM;
                    break;
                default:
                    break;
            }            
        }

        RelicSystem.Instance.OnResourcesHarvested(resourcesOnTileDictionary, resourcesHarvested, tilePosition, out resourcesOnTileDictionary, out resourcesHarvested);
        
        return resourcesHarvested;
    }
    
    public (Dictionary<ResourceType, int>, Dictionary<PersistentResourceType, int>) GetResourceChangeOnTileProcess(Guid cityGuid, Vector2Int tilePosition, Dictionary<ResourceType, int> resourcesSoFar)
    {
        List<TileBuilding> buildingsOnTile = MapSystem.Instance.GetBuildingsOnTile(tilePosition);
        
        Dictionary<ResourceType, int> resourcesProcessed = new Dictionary<ResourceType, int>();
        
        Dictionary<PersistentResourceType, int> persistentResourcesProcessed = new Dictionary<PersistentResourceType, int>();
        
        foreach (TileBuilding building in buildingsOnTile)
        {
            if (HarvestBuildingsController.Instance.CanProcess(building.Type, resourcesSoFar))
            {
                (Dictionary<ResourceType, int>, Dictionary<PersistentResourceType, int>) diff =
                    HarvestBuildingsController.Instance.GetProcessBuildingDiff(building.Type);

                foreach (KeyValuePair<ResourceType, int> resource in diff.Item1)
                {
                    if (!resourcesProcessed.ContainsKey(resource.Key))
                    {
                        resourcesProcessed[resource.Key] = 0;
                    }
                    resourcesProcessed[resource.Key] += resource.Value;
                }
                
                foreach (KeyValuePair<PersistentResourceType, int> persistentResource in diff.Item2)
                {
                    if (!persistentResourcesProcessed.ContainsKey(persistentResource.Key))
                    {
                        persistentResourcesProcessed[persistentResource.Key] = 0;
                    }
                    persistentResourcesProcessed[persistentResource.Key] += persistentResource.Value;
                }
            }
        }
        
        resourcesProcessed = RelicSystem.Instance.OnResourcesProcessed(resourcesProcessed, tilePosition);
        
        persistentResourcesProcessed = RelicSystem.Instance.OnPersistentResourcesProcessed(persistentResourcesProcessed, tilePosition);
        
        return (resourcesProcessed, persistentResourcesProcessed);
    }
}
