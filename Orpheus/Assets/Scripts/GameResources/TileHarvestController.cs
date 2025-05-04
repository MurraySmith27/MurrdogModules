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
                case BuildingType.LumberMill:
                    if (resourcesSoFar[ResourceType.Wood] >= GameConstants.WOOD_PER_LUMBER)
                    {
                        if (!resourcesProcessed.ContainsKey(ResourceType.Lumber))
                        {
                            resourcesProcessed[ResourceType.Lumber] = 0;
                        }
                        resourcesProcessed[ResourceType.Lumber] += GameConstants.LUMBER_PER_LUMBER_MILL;

                        if (!resourcesProcessed.ContainsKey(ResourceType.Wood))
                        {
                            resourcesProcessed[ResourceType.Wood] = 0;
                        }
                        resourcesProcessed[ResourceType.Wood] -= GameConstants.WOOD_PER_LUMBER;
                    }
                    break;
                case BuildingType.CopperYard:
                    if (resourcesSoFar[ResourceType.Stone] >= GameConstants.STONE_PER_COPPER)
                    {
                        if (!resourcesProcessed.ContainsKey(ResourceType.Copper))
                        {
                            resourcesProcessed[ResourceType.Copper] = 0;
                        }
                        resourcesProcessed[ResourceType.Copper] += GameConstants.COPPER_PER_COOPPER_YARD;

                        if (!resourcesProcessed.ContainsKey(ResourceType.Stone))
                        {
                            resourcesProcessed[ResourceType.Stone] = 0;
                        }
                        resourcesProcessed[ResourceType.Stone] -= GameConstants.STONE_PER_COPPER;
                    }
                    break;
                case BuildingType.SteelYard:
                    if (resourcesSoFar[ResourceType.Copper] >= GameConstants.COPPER_PER_STEEL)
                    {
                        if (!resourcesProcessed.ContainsKey(ResourceType.Steel))
                        {
                            resourcesProcessed[ResourceType.Steel] = 0;
                        }
                        resourcesProcessed[ResourceType.Steel] += GameConstants.STEEL_PER_STEEL_YARD;

                        if (!resourcesProcessed.ContainsKey(ResourceType.Copper))
                        {
                            resourcesProcessed[ResourceType.Copper] = 0;
                        }
                        resourcesProcessed[ResourceType.Copper] -= GameConstants.COPPER_PER_STEEL;
                    }
                    break;
                case BuildingType.PopcornFactory:
                    if (resourcesSoFar[ResourceType.Corn] >= GameConstants.CORN_PER_POPCORN)
                    {
                        if (!resourcesProcessed.ContainsKey(ResourceType.Popcorn))
                        {
                            resourcesProcessed[ResourceType.Popcorn] = 0;
                        }
                        resourcesProcessed[ResourceType.Popcorn] += GameConstants.POPCORN_PER_POPCORN_FACTORY;

                        if (!resourcesProcessed.ContainsKey(ResourceType.Corn))
                        {
                            resourcesProcessed[ResourceType.Corn] = 0;
                        }
                        resourcesProcessed[ResourceType.Corn] -= GameConstants.CORN_PER_POPCORN;
                    }
                    break;
                case BuildingType.SushiRestaurant:
                    if (resourcesSoFar[ResourceType.Fish] >= GameConstants.FISH_PER_SUSHI)
                    {
                        if (!resourcesProcessed.ContainsKey(ResourceType.Popcorn))
                        {
                            resourcesProcessed[ResourceType.Popcorn] = 0;
                        }
                        resourcesProcessed[ResourceType.Popcorn] += GameConstants.SUSHI_PER_SUSHI_RESTAURANT;

                        if (!resourcesProcessed.ContainsKey(ResourceType.Fish))
                        {
                            resourcesProcessed[ResourceType.Fish] = 0;
                        }
                        resourcesProcessed[ResourceType.Fish] -= GameConstants.FISH_PER_SUSHI;
                    }
                    break;
            }
        }
        
        resourcesProcessed = RelicSystem.Instance.OnResourcesProcessed(resourcesProcessed, tilePosition);
        
        return resourcesProcessed;
    }
}
