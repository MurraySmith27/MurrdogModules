using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MEC;
using Unity.Mathematics;
using UnityEngine;

public class MapVisualsController : Singleton<MapVisualsController>
{
    [Header("Tile Spawning")]
    [SerializeField] private Transform tileParent;
    [SerializeField] private TilesVisualsSO tilesVisuals;

    [Space(10)]
    [Header("Building Spawning")] 
    [SerializeField] private BuildingsVisualsSO buildingsVisualsData;

    [SerializeField] private float buildingAppearParticleWaitTime = 3f;

    [Space(10)] 
    [Header("Citizen Visualization")] 
    [SerializeField] private CitizenVisualsSO citizenVisualsData;
    
    
    [Space(10)]
    [Header("City Visualization")] 
    [SerializeField] private Transform cityBorderVisualsParent;
    [SerializeField] private CityBorderVisuals cityBorderVisualsPrefab;


    private Dictionary<Vector2Int, CityBorderVisuals> _instantiatedCityBorderVisuals = new();

    public TileVisuals[,] InstantiatedMapTiles = new TileVisuals[0, 0];
    
    public List<(Vector2Int, BuildingBehaviour)> InstantiatedBuildings = new List<(Vector2Int, BuildingBehaviour)>();
    
    public List<(Vector2Int, CitizenBehaviour)> InstantiatedCitizens = new List<(Vector2Int, CitizenBehaviour)>();

    private RectInt _lastCullingRect = new RectInt(new Vector2Int(-1,-1), new Vector2Int(1,1));

    private List<Vector2Int> _grayedOutPositions = new();

    private TileVisuals _lastHoveredOverTileVisuals = null;
    
    private void Start()
    {
        MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
        MapSystem.Instance.OnMapChunkGenerated += OnMapChunkGenerated;

        MapSystem.Instance.OnBuildingConstructed -= OnBuildingConstructed;
        MapSystem.Instance.OnBuildingConstructed += OnBuildingConstructed;
        
        MapSystem.Instance.OnBuildingDestroyed -= OnBuildingDestroyed;
        MapSystem.Instance.OnBuildingDestroyed += OnBuildingDestroyed;

        MapSystem.Instance.OnTilePlaced -= OnTilePlaced;
        MapSystem.Instance.OnTilePlaced += OnTilePlaced;

        MapSystem.Instance.OnCityOwnedTilesChanged -= OnCityOwnedTilesChanged;
        MapSystem.Instance.OnCityOwnedTilesChanged += OnCityOwnedTilesChanged;
        
        MapSystem.Instance.OnTileAddedToCity -= OnTileAddedToCity;
        MapSystem.Instance.OnTileAddedToCity += OnTileAddedToCity;
        
        MapSystem.Instance.OnTileRemovedFromCity -= OnTileRemovedFromCity;
        MapSystem.Instance.OnTileRemovedFromCity += OnTileRemovedFromCity;

        TileFrustrumCulling.Instance.OnTileCullingUpdated -= OnCullingUpdated;
        TileFrustrumCulling.Instance.OnTileCullingUpdated += OnCullingUpdated;
        
        CitizenController.Instance.OnCitizenAddedToTile -= OnCitizenAddedToTile;
        CitizenController.Instance.OnCitizenAddedToTile += OnCitizenAddedToTile;
        
        CitizenController.Instance.OnCitizenRemovedFromTile -= OnCitizenRemovedFromTile;
        CitizenController.Instance.OnCitizenRemovedFromTile += OnCitizenRemovedFromTile;

        MapInteractionController.Instance.OnMapInteractionModeChanged -= OnMapInteractionModeChanged;
        MapInteractionController.Instance.OnMapInteractionModeChanged += OnMapInteractionModeChanged;

        MapInteractionController.Instance.OnTileHoveredOver -= OnTileHoveredOver;
        MapInteractionController.Instance.OnTileHoveredOver += OnTileHoveredOver;

        PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnPhaseChanged;
    }

    private void OnDestroy()
    {
        if (MapSystem.IsAvailable)
        {
            MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
            MapSystem.Instance.OnBuildingConstructed -= OnBuildingConstructed;
            MapSystem.Instance.OnBuildingDestroyed -= OnBuildingDestroyed;
            MapSystem.Instance.OnTilePlaced -= OnTilePlaced;
            MapSystem.Instance.OnCityOwnedTilesChanged -= OnCityOwnedTilesChanged;
            MapSystem.Instance.OnTileAddedToCity -= OnTileAddedToCity;
            MapSystem.Instance.OnTileRemovedFromCity -= OnTileRemovedFromCity;
        }

        if (TileFrustrumCulling.IsAvailable)
        {
            TileFrustrumCulling.Instance.OnTileCullingUpdated -= OnCullingUpdated;
        }

        if (CitizenController.IsAvailable)
        {
            CitizenController.Instance.OnCitizenAddedToTile -= OnCitizenAddedToTile;
            CitizenController.Instance.OnCitizenRemovedFromTile -= OnCitizenRemovedFromTile;
        }

        if (MapInteractionController.IsAvailable)
        {
            MapInteractionController.Instance.OnMapInteractionModeChanged -= OnMapInteractionModeChanged;
            MapInteractionController.Instance.OnTileHoveredOver -= OnTileHoveredOver;
        }
    }

    private void OnPhaseChanged(GamePhases phase)
    {
        if (phase == GamePhases.BloomingUpkeep)
        {
            if (_lastHoveredOverTileVisuals != null)
            {
                _lastHoveredOverTileVisuals.OnHoveredOver(false);
                _lastHoveredOverTileVisuals = null;
            }
        }
    }

    public TileVisuals GetTileInstanceAtPosition(Vector2Int position)
    {
        if (position.x >= InstantiatedMapTiles.GetLength(0) || position.y >= InstantiatedMapTiles.GetLength(0) || position.x < 0 || position.y < 0)
        {
            return null;
        }
        else return InstantiatedMapTiles[position.x, position.y];
    }

    private void OnMapInteractionModeChanged(MapInteractionMode newInteractionMode)
    {
        if (newInteractionMode == MapInteractionMode.PlaceTile)
        {
            ToggleAdjacentTilePreviews(true);
        }
        else
        {
            ToggleAdjacentTilePreviews(false);
        }
    }

    private void OnTileHoveredOver(TileVisuals tileVisuals, Vector2Int tilePosition)
    {
        if (_lastHoveredOverTileVisuals == tileVisuals) return;
        
        if (_lastHoveredOverTileVisuals != null)
        {
            _lastHoveredOverTileVisuals.OnHoveredOver(false);
        }

        if (tileVisuals != null)
        {
            tileVisuals.OnHoveredOver(true);
        }
        
        _lastHoveredOverTileVisuals = tileVisuals;
    }
    
    private void OnMapChunkGenerated(int row, int col, int width, int height)
    {
        //need to populate this map chunk.
        if (row + width > InstantiatedMapTiles.GetLength(0) || row + height > InstantiatedMapTiles.GetLength(1))
        {
            ReallocateMapTilesArray(row + width, col + height);
        }
        
        for (int i = row; i < row + width; i++)
        {
            for (int j = col; j < col + height; j++)
            {
                if (MapSystem.Instance.IsTileOwnedByCity(new Vector2Int(i, j)))
                {
                    InstantiateTileVisuals(i, j);
                }
            }
        }
        
        TileFrustrumCulling.Instance.UpdateTileCulling();
    }

    private void InstantiateTileVisuals(int col, int row)
    {
        TileType tileType = MapSystem.Instance.GetTileType(col, row);

        TileVisualsData tileVisualsData = tilesVisuals.TilesVisualsData.FirstOrDefault(((TileVisualsData data) =>
        {
            return data.Type == tileType;
        }));

        if (tileVisualsData == null)
        {
            Debug.LogError($"Unable to find tile of type {tileType} in tile visuals data SO");
            return;
        }
                
        TileVisuals tilePrefab = tileVisualsData.Prefab;

        InstantiatedMapTiles[col, row] = Instantiate(tilePrefab,
            HexUtils.TileSpaceToWorldSpace(new Vector3(GameConstants.TILE_SIZE * col, 0, GameConstants.TILE_SIZE * row)), Quaternion.identity,
            tileParent);
                
        List<ResourceItem> resourceItems = MapSystem.Instance.GetAllResourcesOnTile(new Vector2Int(col, row));

        InstantiatedMapTiles[col, row].PopulateResourceVisuals(resourceItems);
        
        InstantiatedMapTiles[col, row].ToggleVisuals(false);

        foreach (TileBuilding building in MapSystem.Instance.GetBuildingsOnTile(new Vector2Int(col, row)))
        {
            OnBuildingConstructed(col, row, building.Type);
        }
    }

    private void OnBuildingConstructed(int row, int col, BuildingType buildingType)
    {
        BuildingVisualsData visualData = buildingsVisualsData.BuildingsVisualsData.FirstOrDefault(
            (BuildingVisualsData data) =>
            {
                return data.Type == buildingType;
            });

        if (visualData == null)
        {
            Debug.LogError($"Building visuals data could not be found for type: {buildingType}");
            return;
        }
        
        BuildingBehaviour buildingPrefab = visualData.Prefab;
        
        BuildingBehaviour newBuilding = Instantiate(buildingPrefab,
            HexUtils.TileSpaceToWorldSpace(new Vector3(GameConstants.TILE_SIZE * row, 0, GameConstants.TILE_SIZE * col)), Quaternion.identity,
            tileParent);
        
        InstantiatedBuildings.Add((new Vector2Int(row, col), newBuilding));

        CreateBuildingAppearParticle(newBuilding.transform);

        TileVisuals tile = GetTileInstanceAtPosition(new Vector2Int(row, col));

        if (tile == null)
        {
            Debug.LogError($"Error when constructing building type {newBuilding}: no such tile at position ({row}, {col})");
        }
        else
        {
            tile.AttachBuilding(newBuilding);
        }
        
        CameraController.Instance.FocusPosition(MapUtils.GetTileWorldPositionFromGridPosition(new Vector2Int(row, col)));
    }

    private void CreateBuildingAppearParticle(Transform parent)
    {
        Timing.RunCoroutine(CreateBuildingAppearParticleCoroutine(parent), this.gameObject);
    }

    private IEnumerator<float> CreateBuildingAppearParticleCoroutine(Transform parent)
    {
        GameObject particleInstance = Instantiate(buildingsVisualsData.BuildingAppearParticle, parent);

        yield return Timing.WaitForSeconds(buildingAppearParticleWaitTime);
        
        Destroy(particleInstance);
    }
    
    private void OnBuildingDestroyed(int row, int col)
    {
        Vector2Int position = new(row, col);
        int index = InstantiatedBuildings.FindIndex((pair) =>
        {
            return pair.Item1 == position;
        });
        
        
        if (index != -1)
        {
            TileVisuals tile = GetTileInstanceAtPosition(position);

            if (tile == null)
            {
                Debug.LogError($"Error when destroying building: no such tile at position ({row}, {col})");
            }
            else
            {
                tile.DetachAllBuildings();
                Destroy(InstantiatedBuildings[index].Item2.gameObject);
                InstantiatedBuildings.RemoveAt(index);
            }
        }
        
    }

    private void OnTilePlaced(Vector2Int position, TileInformation tileInformation)
    {
        if (InstantiatedMapTiles[position.x, position.y] != null)
        {
            Destroy(InstantiatedMapTiles[position.x, position.y].gameObject);
        }

        InstantiateTileVisuals(position.x, position.y);
        InstantiatedMapTiles[position.x, position.y].ToggleVisuals(true);
    }

    private void OnCitizenAddedToTile(Guid cityGuid, Vector2Int tilePosition)
    {
        CitizenVisualsData visualData = citizenVisualsData.CitizenVisualsData[0];
        
        CitizenBehaviour buildingPrefab = visualData.Prefab;
        
        CitizenBehaviour newCitizen = Instantiate(buildingPrefab,
            HexUtils.TileSpaceToWorldSpace(new Vector3(GameConstants.TILE_SIZE * tilePosition.x, 0, GameConstants.TILE_SIZE * tilePosition.y)), Quaternion.identity,
            tileParent);
        
        InstantiatedCitizens.Add((tilePosition, newCitizen));
        
        TileVisuals tile = GetTileInstanceAtPosition(tilePosition);

        if (tile == null)
        {
            Debug.LogError($"Error when instantiating citizen: no such tile at position ({tilePosition.x}, {tilePosition.y})");
        }
        else
        {
            tile.AttachCitizen(newCitizen);

            if (_grayedOutPositions.Contains(tilePosition))
            {
                foreach (Vector2Int position in _grayedOutPositions)
                {
                    TileVisuals grayOutTile = GetTileInstanceAtPosition(position);

                    if (grayOutTile != null)
                    {
                        grayOutTile.ToggleGrayOut(false);
                    }
                }
                
                _grayedOutPositions.Clear();
            }
        }
    }

    private void OnCitizenRemovedFromTile(Guid cityGuid, Vector2Int tilePosition)
    {
        TileVisuals tile = GetTileInstanceAtPosition(tilePosition);

        if (tile == null)
        {
            Debug.LogError("tried to remove citizen from tile not owned by city");
            return;
        }

        _grayedOutPositions.Add(tilePosition);
        
        tile.ToggleGrayOut(true);

        if (tile == null)
        {
            Debug.LogError($"Error when detaching citizens: no such tile at position ({tilePosition.x}, {tilePosition.y})");
        }
        else
        {
            for (int i = 0; i < InstantiatedCitizens.Count; i++)
            {
                if (InstantiatedCitizens[i].Item1 == tilePosition)
                {
                    Destroy(InstantiatedCitizens[i].Item2.gameObject);
                    InstantiatedCitizens.RemoveAt(i);
                    break;
                }
                
            }
            
            tile.DetachAllCitizens();
        }
    }

    private void OnCityOwnedTilesChanged(Vector2Int cityCapitalPosition, List<Vector2Int> cityOwnedTiles)
    {
        if (!_instantiatedCityBorderVisuals.ContainsKey(cityCapitalPosition))
        {
            _instantiatedCityBorderVisuals[cityCapitalPosition] = Instantiate(cityBorderVisualsPrefab,
                MapUtils.GetTileWorldPositionFromGridPosition(cityCapitalPosition), Quaternion.identity,
                cityBorderVisualsParent);
        }
        
        _instantiatedCityBorderVisuals[cityCapitalPosition].PopulateCityOwnedTiles(cityOwnedTiles, MapUtils.GetTileWorldPositionFromGridPosition(cityCapitalPosition));

        foreach (Vector2Int cityOwnedTile in cityOwnedTiles)
        {
            InstantiatedMapTiles[cityOwnedTile.x, cityOwnedTile.y].OnTileAppear();
        }
    }

    private void OnTileAddedToCity(Vector2Int cityCapitalPosition, Vector2Int tilePosition)
    {
        CameraController.Instance.FocusPosition(MapUtils.GetTileWorldPositionFromGridPosition(tilePosition));
    }

    private void OnTileRemovedFromCity(Vector2Int cityCapitalPosition, Vector2Int tilePosition)
    {
        ClearTile(tilePosition);
        CameraController.Instance.FocusPosition(MapUtils.GetTileWorldPositionFromGridPosition(tilePosition));
    }

    private void ClearTile(Vector2Int tilePosition)
    {
        TileVisuals tile = GetTileInstanceAtPosition(tilePosition);

        if (tile != null)
        {

            tile.ShadowAppearAnimation();
            tile.DetachAllBuildings();

            foreach (var pair in InstantiatedBuildings)
            {
                if (pair.Item1 == tilePosition)
                {
                    Destroy(pair.Item2.gameObject);
                    InstantiatedBuildings.Remove(pair);
                    break;
                }
            }

            tile.DetachAllCitizens();

            foreach (var pair in InstantiatedCitizens)
            {
                if (pair.Item1 == tilePosition)
                {
                    Destroy(pair.Item2.gameObject);
                    InstantiatedCitizens.Remove(pair);
                    break;
                }
            }
        }
        
        Destroy(tile.gameObject);
    }
    
    private void ReallocateMapTilesArray(int requiredWidth, int requiredHeight)
    {
        int newWidth = Mathf.Max(requiredWidth, InstantiatedMapTiles.GetLength(0));
        int newHeight = Mathf.Max(requiredHeight, InstantiatedMapTiles.GetLength(1));

        TileVisuals[,] temp = new TileVisuals[newWidth, newHeight];
        
        for (int i = 0; i < InstantiatedMapTiles.GetLength(0); i++)
        {
            for (int j = 0; j < InstantiatedMapTiles.GetLength(1); j++)
            {
                temp[i,j] = InstantiatedMapTiles[i,j];
            }
        }
        
        InstantiatedMapTiles = temp;
    }

    private void OnCullingUpdated(int row, int col, int width, int height)
    {
        if (InstantiatedMapTiles.GetLength(0) == 0) return;

        RectInt newCullingRect = new RectInt(row, col, width, height);
        
        if (_lastCullingRect.x >= 0)
        {
            for (int i = _lastCullingRect.x; i < _lastCullingRect.x + _lastCullingRect.width; i++)
            {
                for (int j = _lastCullingRect.y; j < _lastCullingRect.y + _lastCullingRect.height; j++)
                {
                    Vector2Int position = new Vector2Int(i, j);
                    if (!newCullingRect.Contains(position) && i < InstantiatedMapTiles.GetLength(0) && j < InstantiatedMapTiles.GetLength(1) && InstantiatedMapTiles[i, j] != null)
                        InstantiatedMapTiles[i, j].ToggleVisuals(false);
                }
            }
        }
        
        for (int i = row; i < width + row; i++)
        {
            for (int j = col; j < height + col; j++)
            {
                Vector2Int position = new Vector2Int(i, j);
                if (MapSystem.Instance.IsTileOwnedByCity(position) && !_lastCullingRect.Contains(position) && i < InstantiatedMapTiles.GetLength(0) && j < InstantiatedMapTiles.GetLength(1))
                {
                    InstantiatedMapTiles[i, j].ToggleVisuals(true);
                    InstantiatedMapTiles[i, j].ToggleShadow(
                        !MapSystem.Instance.IsTileOwnedByCity(new Vector2Int(i, j))
                    );
                }
            }
        }

        _lastCullingRect = newCullingRect;
    }

    private void ToggleAdjacentTilePreviews(bool toggleEnabled)
    {
        List<Guid> allCityGuids = MapSystem.Instance.GetAllCityGuids();

        if (allCityGuids.Count == 0)
        {
            return;
        }
        
        List<Vector2Int> adjacentTilePositions = MapSystem.Instance.GetValidNewTilePositions(allCityGuids[0]);

        foreach (Vector2Int adjacentTilePosition in adjacentTilePositions)
        {
            TileVisuals tileVisuals = GetTileInstanceAtPosition(adjacentTilePosition);
            
            if (toggleEnabled)
            {
                if (tileVisuals == null)
                {
                    InstantiateTileVisuals(adjacentTilePosition.x, adjacentTilePosition.y);
                }
                
                tileVisuals = GetTileInstanceAtPosition(adjacentTilePosition);

                tileVisuals.TogglePreviewTile(true);
            }
            else
            {
                if (tileVisuals != null)
                {
                    tileVisuals.TogglePreviewTile(false, () =>
                    {
                        ClearTile(adjacentTilePosition);
                    });
                }
            }
        }
    }
}
