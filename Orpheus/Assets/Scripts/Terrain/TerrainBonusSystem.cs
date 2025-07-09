using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBonusSystem : Singleton<TerrainBonusSystem>
{
    private List<(TileType, Vector2Int)> currentCityTiles
    {
        get
        {
            if (!MapSystem.IsAvailable)
            {
                Debug.LogError("Cannot access current city tiles, map system is null");
                return new();
            }

            List<Vector2Int> ownedCityTiles = MapSystem.Instance.GetAllOwnedCityTiles();

            List<(TileType, Vector2Int)> returnVal = new();
            
            foreach (Vector2Int tile in ownedCityTiles)
            {
                returnVal.Add((MapSystem.Instance.GetTileType(tile.x, tile.y), tile));   
            }

            return returnVal;
        }
    } 
    
    private Dictionary<TileType, TerrainBase> _terrainInstances = new Dictionary<TileType, TerrainBase>();
    
    private List<TileType> terrains = new List<TileType>();
    
    private void Awake()
    {
        TerrainFactory terrainFactory = new TerrainFactory();
        
        for (int i = 0; i < Enum.GetValues(typeof(TileType)).Length; i++)
        {
            _terrainInstances.Add((TileType)i, terrainFactory.CreateTerrain((TileType)i));
        }
    }

    public bool GetTileYieldBonuses(out List<(Vector2Int, Vector2Int, int)> yieldBonuses)
    {
        // (source tile, bonus tile, int)
        yieldBonuses = new();

        foreach ((TileType, Vector2Int) cityTile in currentCityTiles)
        {
            List<(Vector2Int, int)> tileBonuses;
            if (_terrainInstances[cityTile.Item1].GetTerrainBonusTileYields(cityTile.Item2, out tileBonuses))
            {
                foreach ((Vector2Int, int) tileBonus in tileBonuses)
                {
                    yieldBonuses.Add((cityTile.Item2, tileBonus.Item1, tileBonus.Item2));
                }
            }
        }

        return yieldBonuses.Count != 0;
    }
}
