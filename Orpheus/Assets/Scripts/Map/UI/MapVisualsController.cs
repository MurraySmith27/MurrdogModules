using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapVisualsController : Singleton<MapVisualsController>
{
    [Header("Tile Spawning")]
    [SerializeField] private Transform tileParent;
    [SerializeField] private TileVisuals waterTilePrefab;
    [SerializeField] private TileVisuals grassTilePrefab;

    [Space(10)] 
    
    [Header("Building Spawning")]
    [SerializeField] private BuildingBehaviour farmPrefab;

    public TileVisuals[,] InstantiatedMapTiles = new TileVisuals[0, 0];
    
    public List<(Vector2Int, BuildingBehaviour)> InstantiatedBuildings = new List<(Vector2Int, BuildingBehaviour)>();

    private RectInt _lastCullingRect = new RectInt(new Vector2Int(-1,-1), new Vector2Int(1,1));
    
    private void Start()
    {
        MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
        MapSystem.Instance.OnMapChunkGenerated += OnMapChunkGenerated;

        MapSystem.Instance.OnBuildingConstructed -= OnBuildingConstructed;
        MapSystem.Instance.OnBuildingConstructed += OnBuildingConstructed;

        TileFrustrumCulling.Instance.OnTileCullingUpdated -= OnCullingUpdated;
        TileFrustrumCulling.Instance.OnTileCullingUpdated += OnCullingUpdated;
    }

    private void OnDestroy()
    {
        if (MapSystem.IsAvailable)
        {
            MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
            MapSystem.Instance.OnBuildingConstructed -= OnBuildingConstructed;
        }

        if (TileFrustrumCulling.IsAvailable)
        {
            TileFrustrumCulling.Instance.OnTileCullingUpdated -= OnCullingUpdated;
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
                

                TileVisuals tilePrefab;
                switch (tileType)
                {
                    case TileType.Water:
                        tilePrefab = waterTilePrefab;
                        break;
                    case TileType.Grass:
                        tilePrefab = grassTilePrefab;
                        break;
                    default:
                        continue;
                }

                InstantiatedMapTiles[i, j] = Instantiate(tilePrefab,
                    new Vector3(GameConstants.TILE_SIZE * i, 0, GameConstants.TILE_SIZE * j), Quaternion.identity,
                    tileParent);
                
                List<ResourceItem> resourceItems = MapSystem.Instance.GetAllResourcesOnTile(new Vector2Int(i, j));

                InstantiatedMapTiles[i, j].PopulateResourceVisuals(resourceItems);

                //start with visuals disabled, change this in camera frustrum culling
                InstantiatedMapTiles[i, j].ToggleVisuals(false);
            }
        }
    }

    private void OnBuildingConstructed(int row, int col, BuildingType buildingType)
    {
        BuildingBehaviour buildingPrefab = null;
        switch (buildingType)
        {
            case BuildingType.CornFarm:
                buildingPrefab = farmPrefab;
                break;
        }
        
        BuildingBehaviour newBuilding = Instantiate(buildingPrefab,
            new Vector3(GameConstants.TILE_SIZE * row, 0, GameConstants.TILE_SIZE * col), Quaternion.identity,
            tileParent);
        
        InstantiatedBuildings.Add((new Vector2Int(row, col), newBuilding));
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
        if (_lastCullingRect.x >= 0)
        {
            for (int i = _lastCullingRect.x; i < _lastCullingRect.x + _lastCullingRect.width; i++)
            {
                for (int j = _lastCullingRect.y; j < _lastCullingRect.y + _lastCullingRect.height; j++)
                {
                    InstantiatedMapTiles[i, j].ToggleVisuals(false);
                }
            }
        }
        
        for (int i = row; i < width + row; i++)
        {
            for (int j = col; j < height + col; j++)
            {
                InstantiatedMapTiles[i, j].ToggleVisuals(true);
            }
        }
        
        _lastCullingRect = new RectInt(row, col, width, height);
    }
}
