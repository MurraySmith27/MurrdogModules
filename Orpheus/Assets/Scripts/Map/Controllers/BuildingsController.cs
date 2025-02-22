using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingsController : Singleton<BuildingsController>
{

    [SerializeField] private BuildingsDataSO buildingsData;
    
    
    public bool TryPlaceBuilding(Vector2Int position, BuildingType type)
    {
        if (!HasBuildingCost(type) || !CanConstructBuildingOnTileType(position, type) ||
            !CanBuildOverExistingStructures(position, type))
        {
            return false;
        }
        
        MapSystem.Instance.ConstructBuilding(position, type);
        return true;
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
}
