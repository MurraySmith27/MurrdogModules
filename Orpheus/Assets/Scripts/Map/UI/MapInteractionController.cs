using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum MapInteractionMode
{
    None,
    Default,
    PlaceBuilding,
    LockCitizens,
    PlaceTile,
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

    public TileInformation CurrentlyPlacingTile
    {
        get;
        private set;
    }

    public event Action<MapInteractionMode> OnMapInteractionModeChanged;

    public event Action<BuildingType> OnPlacingBuildingTypeChanged;

    private MapInteractionMode _lastModeBeforePopup = MapInteractionMode.Default;

    private void Start()
    {
        PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnPhaseChanged;

        UIPopupSystem.Instance.OnPopupShown -= OnPopupShown;
        UIPopupSystem.Instance.OnPopupShown += OnPopupShown;

        UIPopupSystem.Instance.OnPopupHidden -= OnPopupHidden;
        UIPopupSystem.Instance.OnPopupHidden += OnPopupHidden;
    }

    private void OnDestroy()
    {
        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        }

        if (UIPopupSystem.IsAvailable)
        {
            UIPopupSystem.Instance.OnPopupShown -= OnPopupShown;
            UIPopupSystem.Instance.OnPopupHidden -= OnPopupHidden;
        }
    }

    private void OnPhaseChanged(GamePhases gamePhase)
    {
        if (gamePhase == GamePhases.BloomingHarvestTurn)
        {
            SwitchMapInteractionMode(MapInteractionMode.LockCitizens);
        }
        else
        {
            SwitchMapInteractionMode(MapInteractionMode.Default);
        }
    }

    private void OnPopupShown(string popupId)
    {
        if (CurrentMode != MapInteractionMode.None)
        {
            _lastModeBeforePopup = CurrentMode;
        }
        SwitchMapInteractionMode(MapInteractionMode.None);
    }
    
    private void OnPopupHidden(string popupId)
    {
        SwitchMapInteractionMode(_lastModeBeforePopup);
    }

    public void SelectTile(Vector2Int tilePosition)
    {
        _currentlySelectedTile = GetTileFromPosition(tilePosition);
        
        if (_currentlySelectedTile != null)
        {
            switch (CurrentMode)
            {
                case MapInteractionMode.None:
                    break;
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
                case MapInteractionMode.PlaceTile:
                    TryPlaceTile(tilePosition, CurrentlyPlacingTile);
                    break;
                case MapInteractionMode.LockCitizens:
                    if (CitizenController.Instance.IsCitizenOnTile(tilePosition))
                    {
                        CitizenController.Instance.ToggleCitizenAtTileLock(tilePosition);
                    }
                    break;
            }
        }
    }

    public void HoverOverTile(Vector2Int tilePosition)
    {
        _currentlyHoveredOverTile = GetTileFromPosition(tilePosition);
        
        if (_currentlyHoveredOverTile != null)
        {
            switch (CurrentMode)
            {
                case MapInteractionMode.None:
                    break;
                default:
                    if (_currentlySelectedTilePosition != tilePosition)
                    {
                        OnTileHoveredOver?.Invoke(_currentlyHoveredOverTile, tilePosition);
                    }
                    break;
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

    public void SwitchToPlaceTileMode(TileInformation tile)
    {
        CurrentlyPlacingTile = tile;
        
        if (CurrentMode != MapInteractionMode.PlaceTile)
        {
            SwitchMapInteractionMode(MapInteractionMode.PlaceTile);
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
    
    private void TryPlaceTile(Vector2Int tilePosition, TileInformation tile)
    {
        List<Guid> cityGuids = MapSystem.Instance.GetAllCityGuids();
        if (cityGuids.Count == 0)
        {
            Debug.LogError("BUILD A CITY BEFORE PLACING TILES");
        }
        else if (MapSystem.Instance.IsTileOwnedByCity(tilePosition))
        {
            Debug.LogError("CANNOT PLACE TILE ON TILE ALREADY OWNED BY CITY");
        }
        else
        {
            MapSystem.Instance.PlaceTile(tilePosition, tile);
            MapSystem.Instance.AddTileToCity(cityGuids[0], tilePosition);
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
