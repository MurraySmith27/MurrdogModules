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
    private TileBehaviour _currentlySelectedTile = null;

    private Vector2Int _currentlyHoveredOverTilePosition = new Vector2Int(-1, -1);
    private TileBehaviour _currentlyHoveredOverTile = null;

    public event Action<TileBehaviour> OnTileHoveredOver;
    public event Action<TileBehaviour> OnTileSelected;
    
    public MapInteractionMode CurrentMode { get; private set; }= MapInteractionMode.Default;

    private BuildingType _currentlyPlacingBuildingType;

    public event Action<MapInteractionMode> OnMapInteractionModeChanged;

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
                        OnTileSelected?.Invoke(_currentlySelectedTile);
                    }
                    break;
                case MapInteractionMode.PlaceBuilding:
                    TryPlaceBuilding(tilePosition, _currentlyPlacingBuildingType);
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
                OnTileHoveredOver?.Invoke(_currentlyHoveredOverTile);
            }
        }
    }

    public void SwitchMapInteractionMode(MapInteractionMode newMode)
    {
        CurrentMode = newMode;
        
        OnMapInteractionModeChanged?.Invoke(CurrentMode);
    }


    private void TryPlaceBuilding(Vector2Int tilePosition, BuildingType buildingType)
    {
        if (!BuildingsController.Instance.CanBuildOverExistingStructures(tilePosition, buildingType))
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

    private TileBehaviour GetTileFromPosition(Vector2Int position)
    {
        if (!MapVisualsController.IsAvailable)
        {
            Debug.LogError("MapVisualsController is not available!");
            return null; 
        }

        return MapVisualsController.Instance.GetTileInstanceAtPosition(position);
    }
}
