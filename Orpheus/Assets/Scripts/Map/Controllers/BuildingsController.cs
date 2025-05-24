using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingsController : Singleton<BuildingsController>
{
    [SerializeField] private BuildingsDataSO buildingsData;
    
    public bool TryPlaceBuilding(Vector2Int position, BuildingType type)
    {
        BuildingData buildingData = buildingsData.Buildings.FirstOrDefault(data => data.Type == type);

        if (buildingData == null)
        {
            Debug.LogError("Building data is null! cannot place building");
            return false;
        }
        
        if (!HasBuildingCost(type) || !CanConstructBuildingOnTileType(position, type) ||
            !CanBuildOverExistingStructures(position, type))
        {
            return false;
        }

        if (PlayerResourcesSystem.Instance.PayCost(buildingData.Costs))
        {
            MapSystem.Instance.ConstructBuilding(position, type);
            
            RelicSystem.Instance.OnBuildingConstructed(position, type);
            
            return true;
        }
        else return false;
    }

    public bool TryDestroyBuilding(Vector2Int position)
    {
        List<TileBuilding> buildingsOnTile = MapSystem.Instance.GetBuildingsOnTile(position);
        if (buildingsOnTile.Count == 0)
        {
            return false;
        }
        
        if (PlayerResourcesSystem.Instance.PayCost(buildingsData.DestroyBuildingsCost))
        {
            MapSystem.Instance.DestroyBuilding(position);

            foreach (TileBuilding building in buildingsOnTile)
            {
                RelicSystem.Instance.OnBuildingDestroyed(position, building.Type);
            }
            
            return true;
        }
        else return false;
    }

    public bool HasDestroyBuildingCost()
    {
        return PlayerResourcesSystem.Instance.HasCost(buildingsData.DestroyBuildingsCost);
    }

    public List<PersistentResourceItem> GetBuildingCost(BuildingType type)
    {
        BuildingData buildingData = buildingsData.Buildings.FirstOrDefault(data => data.Type == type);

        if (buildingData == null)
        {
            Debug.LogError($"No such building with type {Enum.GetName(typeof(BuildingType), type)}");
            return new();
        }

        return buildingData.Costs;
    }

    public bool HasBuildingCost(BuildingType type)
    {
        BuildingData buildingData = buildingsData.Buildings.FirstOrDefault(data => data.Type == type);

        foreach (PersistentResourceItem cost in buildingData.Costs)
        {
            if (!PlayerResourcesSystem.Instance.HasResource(cost.Type, cost.Quantity))
            {
                return false;
            }
        }

        return true;
    }

    public bool CanConstructBuildingOnTileType(Vector2Int position, BuildingType type)
    {
        TileType tileType = MapSystem.Instance.GetTileType(position.x ,position.y);

        return CanConstructBuildingOnTileType(tileType, type);
    }

    public bool CanConstructBuildingOnTileType(TileType tileType, BuildingType buildingType)
    {
        BuildingData buildingData = buildingsData.Buildings.FirstOrDefault(data => data.Type == buildingType);

        return buildingData != null && buildingData.CanBuildOnTiles.Contains(tileType);
    }

    public bool CanBuildOverExistingStructures(Vector2Int position, BuildingType type)
    {
        List<TileBuilding> existingBuildings = MapSystem.Instance.GetBuildingsOnTile(position);

        //right now, only one building per tile
        if (existingBuildings != null && existingBuildings.Count != 0)
        {
            return false;
        }
        
        return true;
    }

    public bool IsTileOwned(Vector2Int position)
    {
        return MapSystem.Instance.IsTileOwnedByCity(position);
    }
    
    public bool CanBuildCityCapital(Vector2Int position)
    {
        List<Vector2Int> allOwnedCityTiles = MapSystem.Instance.GetAllOwnedCityTiles();

        foreach (Vector2Int tileOffset in GameConstants.INITIAL_CITY_TILES)
        {
            if (allOwnedCityTiles.Contains(position + tileOffset))
            {
                return false;
            }
        }

        return true;
    }

    public List<BuildingType> GetAvailableBuildingTypes()
    {
        List<BuildingType> buildingTypes = new List<BuildingType>(GameConstants.STARTING_BUILDING_TYPES);
        
        buildingTypes.AddRange(TechSystem.Instance.GetUnlockedBuildings());
        
        buildingTypes = RelicSystem.Instance.GetUnlockedBuildingTypes(buildingTypes);
        
        return buildingTypes;
    }
}
