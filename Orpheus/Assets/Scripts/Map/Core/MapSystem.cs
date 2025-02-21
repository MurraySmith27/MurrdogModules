using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
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
    
    private MapGenerator _mapGenerator;
    private TileGrid _tiles = new TileGrid();

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
    
    private void Start()
    {
        List<TileDescriptor> tileDesriptors = AssetManager.GetTileData();
        
        _mapGenerator = new MapGenerator(noiseDensity, cellularAutomataIterations, numAdjacentCellsToMakeLand, tileDesriptors);
    }

    public Vector2Int GetMapDimensions()
    {
        return new Vector2Int(_tiles.GetLength(0), _tiles.GetLength(1));
    }

    //constructs a building on a tile, this doesn't check if the building can be constructed, that should be handled elsewhere.
    public void ConstructBuilding(Vector2Int position, BuildingType buildingType)
    {
        _tiles.AddBuildingToTile(position.x, position.y, buildingType);
        OnBuildingConstructed?.Invoke(position.x, position.y, buildingType);
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
        return _tiles.GetAllResourcesOnTile(position.x, position.y);
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

        TileInformation[,] newTiles = new TileInformation[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                TileInformation newTile = new TileInformation();
                
                newTile.Type = chunk[i,j];
                newTile.Resources = new List<ResourceItem>();
                newTile.Buildings = new List<TileBuilding>();

                newTiles[i, j] = newTile;
            }
        }

        _tiles.AddChunk(x, y, newTiles);

        _onMapChunkGenerated?.Invoke(x, y, width, height);
    }
    
}
