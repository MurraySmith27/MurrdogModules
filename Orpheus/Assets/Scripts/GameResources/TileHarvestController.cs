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
        
        Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
        
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            resources[resourceType] = 0;
        }

        foreach (ResourceItem resourceItem in resourcesOnTile)
        {
            resources[resourceItem.Type] += resourceItem.Quantity;
        }
        
        List<TileBuilding> buildingsOnTile = MapSystem.Instance.GetBuildingsOnTile(tilePosition);

        foreach (TileBuilding building in buildingsOnTile)
        {
            switch (building.Type)
            {
                case BuildingType.CornFarm:
                    resources[ResourceType.Corn] += GameConstants.CORN_PER_CORN_FARM;
                    break;
                
                case BuildingType.WheatFarm:
                    resources[ResourceType.Wheat] += GameConstants.WHEAT_PER_WHEAT_FARM;
                    break;
                
                case BuildingType.FishFarm:
                    resources[ResourceType.Fish] += GameConstants.FISH_PER_FISH_FARM;
                    break;
                
                case BuildingType.Bakery:
                    if (resourcesSoFar[ResourceType.Wheat] >= GameConstants.WHEAT_PER_BREAD)
                    {
                        resources[ResourceType.Bread] += GameConstants.BREAD_PER_BAKERY;
                    }
                    break;
                
                default:
                    break;
            }            
        }

        return resources;
    }
}
