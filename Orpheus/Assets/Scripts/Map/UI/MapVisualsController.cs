using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    private void Start()
    {
        MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
        MapSystem.Instance.OnMapChunkGenerated += OnMapChunkGenerated;

        MapSystem.Instance.OnBuildingConstructed -= OnBuildingConstructed;
        MapSystem.Instance.OnBuildingConstructed += OnBuildingConstructed;

        MapSystem.Instance.OnCityOwnedTilesChanged -= OnCityOwnedTilesChanged;
        MapSystem.Instance.OnCityOwnedTilesChanged += OnCityOwnedTilesChanged;
        
        MapSystem.Instance.OnTileAddedToCity -= OnTileAddedToCity;
        MapSystem.Instance.OnTileAddedToCity += OnTileAddedToCity;

        TileFrustrumCulling.Instance.OnTileCullingUpdated -= OnCullingUpdated;
        TileFrustrumCulling.Instance.OnTileCullingUpdated += OnCullingUpdated;
        
        CitizenController.Instance.OnCitizenAddedToTile -= OnCitizenAddedToTile;
        CitizenController.Instance.OnCitizenAddedToTile += OnCitizenAddedToTile;
        
        CitizenController.Instance.OnCitizenRemovedFromTile -= OnCitizenRemovedFromTile;
        CitizenController.Instance.OnCitizenRemovedFromTile += OnCitizenRemovedFromTile;

        CitizenController.Instance.OnCitizenLocked -= OnCitizenLocked;
        CitizenController.Instance.OnCitizenLocked += OnCitizenLocked;
        
        CitizenController.Instance.OnCitizenUnlocked -= OnCitizenUnlocked;
        CitizenController.Instance.OnCitizenUnlocked += OnCitizenUnlocked;
    }

    private void OnDestroy()
    {
        if (MapSystem.IsAvailable)
        {
            MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
            MapSystem.Instance.OnBuildingConstructed -= OnBuildingConstructed;
            MapSystem.Instance.OnCityOwnedTilesChanged -= OnCityOwnedTilesChanged;
            MapSystem.Instance.OnTileAddedToCity -= OnTileAddedToCity;
        }

        if (TileFrustrumCulling.IsAvailable)
        {
            TileFrustrumCulling.Instance.OnTileCullingUpdated -= OnCullingUpdated;
        }

        if (CitizenController.IsAvailable)
        {
            CitizenController.Instance.OnCitizenAddedToTile -= OnCitizenAddedToTile;
            CitizenController.Instance.OnCitizenRemovedFromTile -= OnCitizenRemovedFromTile;
            CitizenController.Instance.OnCitizenLocked -= OnCitizenLocked;
            CitizenController.Instance.OnCitizenUnlocked -= OnCitizenUnlocked;
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
                TileType tileType = MapSystem.Instance.GetTileType(i, j);

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

                InstantiatedMapTiles[i, j] = Instantiate(tilePrefab,
                    new Vector3(GameConstants.TILE_SIZE * i, 0, GameConstants.TILE_SIZE * j), Quaternion.identity,
                    tileParent);
                
                List<ResourceItem> resourceItems = MapSystem.Instance.GetAllResourcesOnTile(new Vector2Int(i, j));

                InstantiatedMapTiles[i, j].PopulateResourceVisuals(resourceItems);

                //start with visuals disabled, change this in camera frustrum culling
                InstantiatedMapTiles[i, j].ToggleVisuals(false);
            }
        }
        
        TileFrustrumCulling.Instance.UpdateTileCulling();
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
            new Vector3(GameConstants.TILE_SIZE * row, 0, GameConstants.TILE_SIZE * col), Quaternion.identity,
            tileParent);
        
        InstantiatedBuildings.Add((new Vector2Int(row, col), newBuilding));

        TileVisuals tile = GetTileInstanceAtPosition(new Vector2Int(row, col));

        if (tile == null)
        {
            Debug.LogError($"Error when constructing building type {newBuilding}: no such tile at position ({row}, {col})");
        }
        else
        {
            tile.AttachBuilding(newBuilding);
        }
    }

    private void OnCitizenAddedToTile(Guid cityGuid, Vector2Int tilePosition)
    {
        CitizenVisualsData visualData = citizenVisualsData.CitizenVisualsData[0];
        
        CitizenBehaviour buildingPrefab = visualData.Prefab;
        
        CitizenBehaviour newCitizen = Instantiate(buildingPrefab,
            new Vector3(GameConstants.TILE_SIZE * tilePosition.x, 0, GameConstants.TILE_SIZE * tilePosition.y), Quaternion.identity,
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
        }
    }

    private void OnCitizenRemovedFromTile(Guid cityGuid, Vector2Int tilePosition)
    {
        TileVisuals tile = GetTileInstanceAtPosition(tilePosition);

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

    private void OnCitizenLocked(Vector2Int tilePosition)
    {
        TileVisuals tile = GetTileInstanceAtPosition(tilePosition);

        if (tile != null)
        {
            tile.SetCitizenLocked(true);
        }
    }

    private void OnCitizenUnlocked(Vector2Int tilePosition)
    {
        TileVisuals tile = GetTileInstanceAtPosition(tilePosition);

        if (tile != null)
        {
            tile.SetCitizenLocked(false);
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
        
        _instantiatedCityBorderVisuals[cityCapitalPosition].PopulateCityOwnedTiles(cityOwnedTiles);

        foreach (Vector2Int cityOwnedTile in cityOwnedTiles)
        {
            InstantiatedMapTiles[cityOwnedTile.x, cityOwnedTile.y].OnTileAppear();
        }
    }

    private void OnTileAddedToCity(Vector2Int cityCapitalPosition, Vector2Int tilePosition)
    {
        // CameraController.Instance.FocusPosition(MapUtils.GetTileWorldPositionFromGridPosition(tilePosition));
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
                    if (!newCullingRect.Contains(position) && i < InstantiatedMapTiles.GetLength(0) && j < InstantiatedMapTiles.GetLength(1))
                        InstantiatedMapTiles[i, j].ToggleVisuals(false);
                }
            }
        }
        
        for (int i = row; i < width + row; i++)
        {
            for (int j = col; j < height + col; j++)
            {
                Vector2Int position = new Vector2Int(i, j);
                if (!_lastCullingRect.Contains(position) && i < InstantiatedMapTiles.GetLength(0) && j < InstantiatedMapTiles.GetLength(1))
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
}
