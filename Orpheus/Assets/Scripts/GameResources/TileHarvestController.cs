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
        
        //do harvest
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            resourcesHarvested[resourceType] = 0;
        }

        foreach (ResourceItem resourceItem in resourcesOnTile)
        {
            resourcesHarvested[resourceItem.Type] += resourceItem.Quantity;
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

        resourcesHarvested = RelicSystem.Instance.OnResourcesHarvested(resourcesHarvested, tilePosition);
        
        
        return resourcesHarvested;
    }
    
    public Dictionary<ResourceType, int> GetResourceChangeOnTileProcess(Guid cityGuid, Vector2Int tilePosition, Dictionary<ResourceType, int> resourcesSoFar)
    {
        List<TileBuilding> buildingsOnTile = MapSystem.Instance.GetBuildingsOnTile(tilePosition);
        
        Dictionary<ResourceType, int> resourcesProcessed = new Dictionary<ResourceType, int>();
        
        foreach (TileBuilding building in buildingsOnTile)
        {
            switch (building.Type)
            {
                case BuildingType.Bakery:
                    if (resourcesSoFar[ResourceType.Wheat] >= GameConstants.WHEAT_PER_BREAD)
                    {
                        if (!resourcesProcessed.ContainsKey(ResourceType.Bread))
                        {
                            resourcesProcessed[ResourceType.Bread] = 0;
                        }
                        resourcesProcessed[ResourceType.Bread] += GameConstants.BREAD_PER_BAKERY;

                        if (!resourcesProcessed.ContainsKey(ResourceType.Wheat))
                        {
                            resourcesProcessed[ResourceType.Wheat] = 0;
                        }
                        resourcesProcessed[ResourceType.Wheat] -= GameConstants.WHEAT_PER_BREAD;
                    }
                    break;
            }
        }
        
        resourcesProcessed = RelicSystem.Instance.OnResourcesProcessed(resourcesProcessed, tilePosition);
        
        return resourcesProcessed;
    }
}
