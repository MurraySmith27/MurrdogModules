using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class MapSystem : Singleton<MapSystem>
{
    [Header("Map Generation Settings")] 
    [SerializeField] private Vector2Int mapDimensions;
    [SerializeField] private float noiseDensity = 0.5f;
    [SerializeField] private int cellularAutomataIterations = 5;
    [SerializeField] private int numAdjacentCellsToMakeLand = 4;
    
    [Space(10)]
    
    [Header("Resource Population Settings")]
    [SerializeField] private float cornProbabilityOnGrassTile = 0.2f;
    [SerializeField] private float wheatProbabilityOnGrassTile = 0.2f;
    [SerializeField] private float fishProbabilityOnWaterTile = 0.4f;
    [SerializeField] private float woodProbabilityOnGrassTile = 0.6f;
    [SerializeField] private float stoneProbabilityOnGrassTile = 0.4f;
    [SerializeField] private float woodProbabilityOnWaterTile = 0.6f;
    [SerializeField] private float stoneProbabilityOnWaterTile = 0.4f;

    [Header("Resources Settings")] 
    [SerializeField] private bool resourcesGeneratedAtStart = false;
    
    private MapGenerator _mapGenerator;
    private MapResourcesGenerator _mapResourcesGenerator;
    private TileGrid _tiles = new TileGrid();

    private List<CityTileData> _cities = new List<CityTileData>();

    public delegate void OnMapChunkGeneratedDelegate(int row, int col, int width, int height); 
    
    private event OnMapChunkGeneratedDelegate _onMapChunkGenerated;
    
    public event OnMapChunkGeneratedDelegate OnMapChunkGenerated
    {
        add
        {
            if (_tiles.GetLength(0) > 0)
            {
                value(0, 0, _tiles.GetLength(0), _tiles.GetLength(1));
            }
            
            _onMapChunkGenerated += value;
        }
        remove
        {
            _onMapChunkGenerated -= value;
        }
    }

    public delegate void OnBuildingConstructedDelegate(int row, int col, BuildingType building);
    
    public event OnBuildingConstructedDelegate OnBuildingConstructed;

    public delegate void OnCityOwnedTilesChangedDelegate(Vector2Int cityCapitalLocation,
        List<Vector2Int> cityOwnedTiles);
    
    public event OnCityOwnedTilesChangedDelegate OnCityOwnedTilesChanged;

    public delegate void OnTileAddedOrRemovedFromCityDelegate(Vector2Int cityCapitalLocation, Vector2Int tilePosition);
    
    public event OnTileAddedOrRemovedFromCityDelegate OnTileAddedToCity;
    
    public event OnTileAddedOrRemovedFromCityDelegate OnTileRemovedFromCity;
    
    public delegate void OnTileResourcesChangedDelegate(Vector2Int position, ResourceType resourceType, int difference);
    public event OnTileResourcesChangedDelegate OnTileResourcesChanged;
    
    public delegate void OnTilePlacedDelegate(Vector2Int tilePosition, TileInformation tile);

    public event OnTilePlacedDelegate OnTilePlaced;
    
    private void Start()
    {
        List<TileDescriptor> tileDesriptors = AssetManager.GetTileData();
        
        _mapGenerator = new MapGenerator(
            noiseDensity, 
            cellularAutomataIterations, 
            numAdjacentCellsToMakeLand, 
            tileDesriptors
        );

        _mapResourcesGenerator = new MapResourcesGenerator(
            cornProbabilityOnGrassTile,
            wheatProbabilityOnGrassTile,
            fishProbabilityOnWaterTile,
            woodProbabilityOnGrassTile,
            stoneProbabilityOnGrassTile,
            woodProbabilityOnWaterTile,
            stoneProbabilityOnWaterTile
        );
    }

    public Vector2Int GetMapDimensions()
    {
        return new Vector2Int(_tiles.GetLength(0), _tiles.GetLength(1));
    }

    public void AddStartingCity()
    {
        Vector2Int startingCityPos = new Vector2Int(Mathf.FloorToInt(GameConstants.STARTING_MAP_SIZE.x / 2f),
            Mathf.FloorToInt(GameConstants.STARTING_MAP_SIZE.y / 2f));
        
        for (int i = 0; i < GameConstants.INITIAL_CITY_TILES.Length; i++)
        {
            TileInformation tile = new TileInformation();

            tile.Buildings = new();
            tile.Resources = new();
            if (GameConstants.INITIAL_CITY_RESOURCES[i].Quantity > 0)
            {
                tile.Resources.Add(GameConstants.INITIAL_CITY_RESOURCES[i]);
            }
            tile.Type = GameConstants.INITIAL_CITY_TILE_TYPES[i];
            
            PlaceTile(startingCityPos + GameConstants.INITIAL_CITY_TILES[i], tile);
        }
        
        ConstructBuilding(startingCityPos, BuildingType.CityCapital);
    }

    //constructs a building on a tile, this doesn't check if the building can be constructed, that should be handled elsewhere.
    public void ConstructBuilding(Vector2Int position, BuildingType buildingType)
    {
        _tiles.AddBuildingToTile(position.x, position.y, buildingType);

        if (buildingType == BuildingType.CityCapital)
        {

            List<Vector2Int> initialTileLocations = GameConstants.INITIAL_CITY_TILES.Select((Vector2Int offset) =>
            {
                return offset + position;
            }).ToList();
            
            CityTileData newCity = new CityTileData(position, new());
            
            _cities.Add(newCity);

            foreach (Vector2Int tileLocation in initialTileLocations)
            {
                TryAddTileToCity(newCity.CityGuid, tileLocation, true);
            }

            OnBuildingConstructed?.Invoke(position.x, position.y, buildingType);
            OnCityOwnedTilesChanged?.Invoke(position, initialTileLocations);
        }
        else
        {
            OnBuildingConstructed?.Invoke(position.x, position.y, buildingType);
        }
    }

    public void PlaceTile(Vector2Int position, TileInformation tileInformation)
    {
        _tiles.SwapOutTile(position.x, position.y, tileInformation);
        
        OnTilePlaced?.Invoke(position, tileInformation);
    }

    public List<Vector2Int> GetAllOwnedCityTiles()
    {
        List<Vector2Int> allCityTiles = new List<Vector2Int>();
        foreach (CityTileData cityTileData in _cities)
        {
            allCityTiles.AddRange(cityTileData.GetTilesInOrder());
        }

        return allCityTiles;
    }

    public List<Guid> GetAllCityGuids()
    {
        return _cities.Select((CityTileData cityTileData) => cityTileData.CityGuid).ToList();
    }

    public bool GetCityGuidFromTile(Vector2Int position, out Guid cityGuid)
    {
        cityGuid = new();
        
        foreach (CityTileData city in _cities)
        {
            if (city.GetTilesInOrder().Contains(position))
            {
                cityGuid = city.CityGuid;
                return true;
            }
        }

        return false;
    }

    public List<Vector2Int> GetOwnedTilesOfCity(Guid cityGuid)
    {
        CityTileData city = _cities.FirstOrDefault((CityTileData data) => data.CityGuid == cityGuid);

        if (city != null)
        {
            return city.GetTilesInOrder();
        }
        else return new();
    }

    public Vector2Int GetCityCenterPosition(Guid cityGuid)
    {
        CityTileData city = _cities.FirstOrDefault((CityTileData data) => data.CityGuid == cityGuid);
        
        if (city == null)
        {
            Debug.LogError($"Error in GetCityCenterPosition: No such city with guid {cityGuid} exists!");
            return new Vector2Int(0, 0);
        }
        else return city.GetCapitalLocation();
    }

    public bool IsTileOwnedByCity(Vector2Int position)
    {
        return _cities.FirstOrDefault((CityTileData city) =>
        {
            return city.IsLocationInCity(position);
        }) != null;
    }

    public List<Vector2Int> GetCityAdjacentTiles(Guid cityGuid)
    {
        CityTileData city = _cities.FirstOrDefault((CityTileData city) =>
        {
            return city.CityGuid == cityGuid;
        });

        if (city == null)
        {
            Debug.LogError($"No such city with guid: {cityGuid} exists in tile data");
            return new();
        }

        List<Vector2Int> cityTiles = city.GetTilesInOrder();
        
        HashSet<Vector2Int> adjacentTiles = new();

        Vector2Int[] offsets = new Vector2Int[6]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, -1)
        };
        
        foreach (Vector2Int tileLocation in cityTiles)
        {
            foreach (Vector2Int offset in offsets)
            {
                if (!adjacentTiles.Contains(tileLocation + offset) && IsTileAdjacentToCity(tileLocation + offset))
                {
                    adjacentTiles.Add(tileLocation + offset);
                }
            }
        }
        
        return adjacentTiles.ToList();
    }

    public bool IsTileAdjacentToCity(Vector2Int position)
    {
        return _cities.FirstOrDefault((CityTileData city) =>
        {
            return !city.IsLocationInCity(position) && (
                city.IsLocationInCity(position + new Vector2Int(1, 0)) || 
                city.IsLocationInCity(position + new Vector2Int(-1, 0)) ||
                city.IsLocationInCity(position + new Vector2Int(0, 1)) ||
                city.IsLocationInCity(position + new Vector2Int(-1, 1)) || 
                city.IsLocationInCity(position + new Vector2Int(0, -1)) || 
                city.IsLocationInCity(position + new Vector2Int(1, -1))
                );
        }) != null;
    }

    public Vector2Int AddRandomTileToCity(Guid cityGuid)
    {
        HashSet<Vector2Int> extremumTiles = new HashSet<Vector2Int>();
        
        CityTileData city = _cities.FirstOrDefault((CityTileData data) => data.CityGuid == cityGuid);

        if (city == null)
        {
            Debug.LogError($"Cannot find city with guid: {cityGuid}");
            return new Vector2Int(-1, -1);
        }
        
        Vector2Int cityCenterPosition = city.GetCapitalLocation();
        
        List<Vector2Int> ownedCityTiles = city.GetTilesInOrder();

        foreach (Vector2Int ownedCityTile in ownedCityTiles)
        {
            extremumTiles.Add(ownedCityTile + new Vector2Int(0, 1));
            extremumTiles.Add(ownedCityTile + new Vector2Int(0, -1));
            extremumTiles.Add(ownedCityTile + new Vector2Int(1, 0));
            extremumTiles.Add(ownedCityTile + new Vector2Int(-1, 0));
        }
        
        foreach (Vector2Int ownedCityTile in ownedCityTiles)
        {
            extremumTiles.Remove(ownedCityTile);
        }
        
        Dictionary<int, List<Vector2Int>> taxicabDistancePositions = new Dictionary<int, List<Vector2Int>>();

        int minDistance = int.MaxValue;
        
        foreach (Vector2Int ownedTile in extremumTiles)
        {
            int taxicabDistance = Math.Abs(ownedTile.x - cityCenterPosition.x) + Math.Abs(ownedTile.y - cityCenterPosition.y);
            
            if (!taxicabDistancePositions.ContainsKey(taxicabDistance))
            {
                taxicabDistancePositions.Add(taxicabDistance, new List<Vector2Int>());
            }
            
            taxicabDistancePositions[taxicabDistance].Add(ownedTile);

            if (taxicabDistance < minDistance)
            {
                minDistance = taxicabDistance;
            }
        }

        Vector2Int newTile = taxicabDistancePositions[minDistance][UnityEngine.Random.Range(0, taxicabDistancePositions[minDistance].Count)];

        if (TryAddTileToCity(cityGuid, newTile, true))
        {
            OnCityOwnedTilesChanged?.Invoke(cityCenterPosition, city.GetTilesInOrder());
            return newTile;
        }
        else return new Vector2Int(-1, -1);
    }

    public void AddTileToCity(Guid cityGuid, Vector2Int tilePosition, bool generateResources = false)
    {
        if (!TryAddTileToCity(cityGuid, tilePosition, generateResources))
        {
            Debug.LogError($"Failed to add tile: {tilePosition} to city with guid: {cityGuid}.");
            return;
        }
        else
        {
            CityTileData cityData = _cities.FirstOrDefault((CityTileData data) =>
            {
                return data.CityGuid == cityGuid;
            });
            
            Vector2Int cityCapitalLocation = cityData!.GetCapitalLocation();
            
            OnCityOwnedTilesChanged?.Invoke(cityCapitalLocation, cityData!.GetTilesInOrder());
        }
    }

    private bool TryAddTileToCity(Guid cityGuid, Vector2Int newTilePosition, bool addResources = false)
    {
        CityTileData city = _cities.FirstOrDefault((CityTileData data) => data.CityGuid == cityGuid);

        if (city == null)
        {
            Debug.LogError($"Cannot find city with guid: {cityGuid}");
            return false;
        }
        
        Vector2Int cityCenterPosition = city.GetCapitalLocation();
        
        city.AddTileToCity(newTilePosition);
        
        // if (!resourcesGeneratedAtStart)
        // {
        //     TileType type = _tiles[newTilePosition.x, newTilePosition.y].Type;
        //     if (addResources)
        //     {
        //         List<ResourceItem> resources = GenerateResourcesOnTile(type);
        //
        //         for (int i = 0; i < resources.Count; i++)
        //         {
        //             _tiles.AddResourceToTile(newTilePosition.x, newTilePosition.y, resources[i].Type,
        //                 resources[i].Quantity);
        //             OnTileResourcesChanged?.Invoke(newTilePosition, resources[i].Type, resources[i].Quantity);
        //         }
        //     }
        // }
        
        OnTileAddedToCity?.Invoke(cityCenterPosition, newTilePosition);
        
        return true;
    }

    public bool TryRemoveTileFromCity(Vector2Int tilePosition)
    {
        Guid cityGuid;
        if (!GetCityGuidFromTile(tilePosition, out cityGuid))
        {
            Debug.LogError($"Cannot remove tile from city. could not find city that owns tile ({tilePosition.x}, {tilePosition.y}).");
            return false;
        }
        
        CityTileData tileData = _cities.FirstOrDefault((CityTileData data) => data.CityGuid == cityGuid);
        if (tileData == null)
        {
            Debug.LogError(
                $"$Cannot remove tile from city. Could not find city with guid {cityGuid} in city tile data.");
            return false;
        }

        if (tileData.IsLocationInCity(tilePosition))
        {
            tileData.RemoveTileFromCity(tilePosition);
            
            OnTileRemovedFromCity?.Invoke(tileData.GetCapitalLocation(), tilePosition);
            
            OnCityOwnedTilesChanged?.Invoke(tileData.GetCapitalLocation(), tileData.GetTilesInOrder());
            
            return true;
        }
        else return false;
    }

    public List<ResourceItem> GenerateResourcesOnTile(TileType type)
    {
        return _mapResourcesGenerator.GenerateResourcesOnTile(type);
    }

    public bool GetUnoccupiedTileInCity(Guid cityGuid, out Vector2Int location)
    {
        location = new Vector2Int();
        
        CityTileData cityTileData = _cities.FirstOrDefault((CityTileData data) => data.CityGuid == cityGuid);

        if (cityTileData == null)
        {
            Debug.LogError($"no such city with id: {cityGuid} exists!");
            return false;
        }
        
        List<Vector2Int> ownedCityTiles = cityTileData.GetTilesInOrder();
        
        List<Vector2Int> unoccupiedTiles = ownedCityTiles.Where(
            (Vector2Int tile) =>
            {
                return _tiles.GetAllBuildingsOnTile(tile.x, tile.y).Count > 0;
            }).ToList();

        if (unoccupiedTiles.Count > 0)
        {
            location = unoccupiedTiles[UnityEngine.Random.Range(0, unoccupiedTiles.Count)];
            return true;
        }
        else return false;
    }

    public List<TileBuilding> GetBuildingsOnTile(Vector2Int position)
    {
        return _tiles.GetAllBuildingsOnTile(position.x, position.y);
    }
    
    public void AddResourcesToTile(Vector2Int position, ResourceType resourceType, int quantity)
    {
        _tiles.AddResourceToTile(position.x, position.y, resourceType, quantity);

        OnTileResourcesChanged?.Invoke(position, resourceType, quantity);
    }

    public List<ResourceItem> GetAllResourcesOnTile(Vector2Int position)
    {
        if (_tiles.ValidPosition(position.x, position.y))
        {
            return _tiles.GetAllResourcesOnTile(position.x, position.y);
        }
        else return new();
    }

    public TileType GetTileType(int col, int row)
    {
        TileInformation? tileInformation = _tiles[col, row];

        return tileInformation.GetValueOrDefault().Type;
    }

    public void GenerateMapChunk(int x, int y, int width, int height)
    {
        if (width == 0 || height == 0)
        {
            Debug.LogError($"Attempting to generate a chunk with width: {width} and height {height}. This is not allowed");
            return;
        }

        //TODO: eventually, generate the chunk based on the surrounding map data, add padding so inital map is generated far out from the origin.
        TileType[,] chunk = _mapGenerator.GenerateMap(width, height, RandomChanceSystem.Instance.GetCurrentSeed());

        if (chunk.GetLength(0) != width || chunk.GetLength(1) != height)
        {
            Debug.LogError(
                $"Error when generating chunk! the chunk recieved from the map generator does not match the provided dimenstions. " +
                $"Provided dimensions: ({width}, {height}), generated chunk dimensions: ({chunk.GetLength(0)}, {chunk.GetLength(1)})");
            return;
        }

        List<ResourceItem>[,] resources;
        if (resourcesGeneratedAtStart)
        {
            resources = _mapResourcesGenerator.GenerateResourcesOnChunk(chunk, RandomChanceSystem.Instance.GetCurrentSeed());

            if (resources.GetLength(0) != width || resources.GetLength(1) != height)
            {
                Debug.LogError(
                    "Resources generated dimensions is not equal to the chunk dimensions! something has gone wrong." +
                    $"resources list dimensions: ({resources.GetLength(0)}, {resources.GetLength(1)})," +
                    $" generated chunk dimensions: ({chunk.GetLength(0)}, {chunk.GetLength(1)})");
                return;
            }
        }
        else
        {
            resources = new List<ResourceItem>[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    resources[i, j] = new List<ResourceItem>();
                }
            }
        }

        TileInformation[,] newTiles = new TileInformation[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                TileInformation newTile = new TileInformation();
                
                newTile.Type = chunk[i,j];
                newTile.Resources = resources[i,j];
                newTile.Buildings = new List<TileBuilding>();

                newTiles[i, j] = newTile;
            }
        }

        _tiles.AddChunk(x, y, newTiles);

        _onMapChunkGenerated?.Invoke(x, y, width, height);
    }
    
}
