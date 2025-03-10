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
            return true;
        }
        else return false;
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
        
        BuildingData buildingData = buildingsData.Buildings.FirstOrDefault(data => data.Type == type);

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
}
