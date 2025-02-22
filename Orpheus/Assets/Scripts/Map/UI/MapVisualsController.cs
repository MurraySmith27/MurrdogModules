using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapVisualsController : Singleton<MapVisualsController>
{
    [Header("Tile Spawning")]
    [SerializeField] private Transform tileParent;
    [SerializeField] private TileBehaviour waterTilePrefab;
    [SerializeField] private TileBehaviour grassTilePrefab;

    [Space(10)] 
    
    [Header("Building Spawning")]
    [SerializeField] private BuildingBehaviour farmPrefab;

    public TileBehaviour[,] InstantiatedMapTiles = new TileBehaviour[0, 0];
    
    public List<(Vector2Int, BuildingBehaviour)> InstantiatedBuildings = new List<(Vector2Int, BuildingBehaviour)>();
    
    private void Start()
    {
        MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
        MapSystem.Instance.OnMapChunkGenerated += OnMapChunkGenerated;

        MapSystem.Instance.OnBuildingConstructed -= OnBuildingConstructed;
        MapSystem.Instance.OnBuildingConstructed += OnBuildingConstructed;
    }

    private void OnDestroy()
    {
        if (MapSystem.IsAvailable)
        {
            MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
            MapSystem.Instance.OnBuildingConstructed -= OnBuildingConstructed;
        }
    }

    public TileBehaviour GetTileInstanceAtPosition(Vector2Int position)
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
                

                TileBehaviour tilePrefab;
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

        TileBehaviour[,] temp = new TileBehaviour[newWidth, newHeight];
        
        for (int i = 0; i < InstantiatedMapTiles.GetLength(0); i++)
        {
            for (int j = 0; j < InstantiatedMapTiles.GetLength(1); j++)
            {
                temp[i,j] = InstantiatedMapTiles[i,j];
            }
        }
        
        InstantiatedMapTiles = temp;
    }
}
