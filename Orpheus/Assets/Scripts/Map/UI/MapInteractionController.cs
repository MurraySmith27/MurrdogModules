using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum MapInteractionMode
{
    Default,
    PlaceBuilding,
}

public class MapInteractionController : Singleton<MapInteractionController>
{
    private Vector2Int _currentlySelectedTilePosition = new Vector2Int(-1, -1);
    private TileVisuals _currentlySelectedTile = null;

    private Vector2Int _currentlyHoveredOverTilePosition = new Vector2Int(-1, -1);
    private TileVisuals _currentlyHoveredOverTile = null;

    public event Action<TileVisuals, Vector2Int> OnTileHoveredOver;
    public event Action<TileVisuals, Vector2Int> OnTileSelected;
    
    public MapInteractionMode CurrentMode { get; private set; } = MapInteractionMode.Default;

    public BuildingType CurrentlyPlacingBuildingType
    {
        get;
        private set;
    }

    public event Action<MapInteractionMode> OnMapInteractionModeChanged;

    public event Action<BuildingType> OnPlacingBuildingTypeChanged;

    public void SelectTile(Vector2Int tilePosition)
    {
        _currentlySelectedTile = GetTileFromPosition(tilePosition);
        
        if (_currentlySelectedTile != null)
        {
            switch (CurrentMode)
            {
                case MapInteractionMode.Default:
                    if (_currentlySelectedTilePosition != tilePosition)
                    {
                        _currentlySelectedTilePosition = tilePosition;
                        OnTileSelected?.Invoke(_currentlySelectedTile, tilePosition);
                    }
                    break;
                case MapInteractionMode.PlaceBuilding:
                    TryPlaceBuilding(tilePosition, CurrentlyPlacingBuildingType);
                    break;
            }
        }
    }

    public void HoverOverTile(Vector2Int tilePosition)
    {
        _currentlyHoveredOverTile = GetTileFromPosition(tilePosition);
        
        if (_currentlyHoveredOverTile != null)
        {
            if (_currentlySelectedTilePosition != tilePosition)
            {
                OnTileHoveredOver?.Invoke(_currentlyHoveredOverTile, tilePosition);
            }
        }
    }

    public void SwitchToPlaceBuildingMode(BuildingType buildingType)
    {
        bool wasInDifferentBuildingType = CurrentlyPlacingBuildingType != buildingType;
        if (wasInDifferentBuildingType)
        {
            CurrentlyPlacingBuildingType = buildingType;
        }
        if (CurrentMode != MapInteractionMode.PlaceBuilding)
        {
            SwitchMapInteractionMode(MapInteractionMode.PlaceBuilding);
        }

        if (wasInDifferentBuildingType)
        {
            OnPlacingBuildingTypeChanged?.Invoke(buildingType);
        }
    }
    
    public void SwitchMapInteractionMode(MapInteractionMode newMode)
    {
        CurrentMode = newMode;
        
        OnMapInteractionModeChanged?.Invoke(CurrentMode);
    }

    private void TryPlaceBuilding(Vector2Int tilePosition, BuildingType buildingType)
    {
        if (buildingType == BuildingType.CityCapital && !BuildingsController.Instance.CanBuildCityCapital(tilePosition))
        {
            Debug.LogError("CANNOT BUILD CITY TOO CLOSE TO OTHER CITY");
        }
        else if (buildingType != BuildingType.CityCapital && !BuildingsController.Instance.IsTileOwned(tilePosition))
        {
            Debug.LogError("CANNOT BUILD BUILDING ON TILE THAT ISN'T OWNED BY A CITY");   
        }
        else if (!BuildingsController.Instance.CanBuildOverExistingStructures(tilePosition, buildingType))
        {
            Debug.LogError("CANNOT BUILD BUILDING ON TILE WITH EXISTING STRUCTURE");
        }
        else if (!BuildingsController.Instance.CanConstructBuildingOnTileType(tilePosition, buildingType))
        {
            Debug.LogError("CANNOT BUILD BUILDING OF THIS TYPE ON THIS TILE");
        }
        else if (!BuildingsController.Instance.HasBuildingCost(buildingType))
        {
            Debug.LogError("DOESN'T HAVE BUILDING COST");
        }
        else
        {
            BuildingsController.Instance.TryPlaceBuilding(tilePosition, buildingType);
        }
    }

    private TileVisuals GetTileFromPosition(Vector2Int position)
    {
        if (!MapVisualsController.IsAvailable)
        {
            Debug.LogError("MapVisualsController is not available!");
            return null; 
        }

        return MapVisualsController.Instance.GetTileInstanceAtPosition(position);
    }

    public Vector2Int GetCurrentlySelectedTile()
    {
        return _currentlySelectedTilePosition;
    }
}
