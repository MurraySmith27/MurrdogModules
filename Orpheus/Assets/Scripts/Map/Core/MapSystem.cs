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

    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int mapSeed = 0;
    
    [Space(10)]
    
    [Header("Resource Population Settings")]
    [SerializeField] private float cornProbabilityOnGrassTile = 0.2f;
    [SerializeField] private float wheatProbabilityOnGrassTile = 0.2f;
    [SerializeField] private float fishProbabilityOnWaterTile = 0.4f;
    [SerializeField] private float woodProbabilityOnGrassTile = 0.6f;
    [SerializeField] private float stoneProbabilityOnGrassTile = 0.4f;
    [SerializeField] private float woodProbabilityOnWaterTile = 0.6f;
    [SerializeField] private float stoneProbabilityOnWaterTile = 0.4f;
    
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
            
            CityTileData newCity = new CityTileData(position, initialTileLocations);
            
            _cities.Add(newCity);
            
            OnBuildingConstructed?.Invoke(position.x, position.y, buildingType);
            OnCityOwnedTilesChanged?.Invoke(position, initialTileLocations);
        }
        else
        {
            OnBuildingConstructed?.Invoke(position.x, position.y, buildingType);
        }
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

    public List<Vector2Int> GetOwnedTilesOfCity(Guid cityGuid)
    {
        CityTileData city = _cities.FirstOrDefault((CityTileData data) => data.CityGuid == cityGuid);

        if (city != null)
        {
            return city.GetTilesInOrder();
        }
        else return new();
    }

    public bool IsTileOwnedByCity(Vector2Int position)
    {
        return _cities.FirstOrDefault((CityTileData city) =>
        {
            return city.IsLocationInCity(position);
        }) != null;
    }

    public List<TileBuilding> GetBuildingsOnTile(Vector2Int position)
    {
        return _tiles.GetAllBuildingsOnTile(position.x, position.y);
    }
    
    public void AddResourcesToTile(Vector2Int position, ResourceType resourceType, int quantity)
    {
        _tiles.AddResourceToTile(position.x, position.y, resourceType, quantity);
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
        
        int seed = mapSeed;
        if (useRandomSeed)
            seed = UnityEngine.Random.Range(Int32.MinValue, Int32.MaxValue);

        //TODO: eventually, generate the chunk based on the surrounding map data, add padding so inital map is generated far out from the origin.
        TileType[,] chunk = _mapGenerator.GenerateMap(width, height, seed);

        if (chunk.GetLength(0) != width || chunk.GetLength(1) != height)
        {
            Debug.LogError(
                $"Error when generating chunk! the chunk recieved from the map generator does not match the provided dimenstions. " +
                $"Provided dimensions: ({width}, {height}), generated chunk dimensions: ({chunk.GetLength(0)}, {chunk.GetLength(1)})");
            return;
        }
        
        List<ResourceItem>[,] resources = _mapResourcesGenerator.GenerateResourceOnChunk(chunk, seed);

        if (resources.GetLength(0) != width || resources.GetLength(1) != height)
        {
            Debug.LogError("Resources generated dimensions is not equal to the chunk dimensions! something has gone wrong." +
                           $"resources list dimensions: ({resources.GetLength(0)}, {resources.GetLength(1)})," +
                           $" generated chunk dimensions: ({chunk.GetLength(0)}, {chunk.GetLength(1)})");
            return;
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
