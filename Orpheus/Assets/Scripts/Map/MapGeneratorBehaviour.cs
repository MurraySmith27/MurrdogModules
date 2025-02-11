using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGeneratorBehaviour : Singleton<MapGeneratorBehaviour>
{
    
    
    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private TileDataSO tileData;
    [SerializeField] private Transform tileParent;
    
    [Header("Cellular Automata Settings")]
    [SerializeField] private float noiseDensity = 0.5f;
    [SerializeField] private int cellularAutomataIterations = 5;
    [SerializeField] private int numAdjacentCellsToMakeLand = 4;
    
    void Start()
    {
        MapGenerator mapGenerator = new MapGenerator(noiseDensity, cellularAutomataIterations, numAdjacentCellsToMakeLand, tileData.tiles);
        char[,] map = mapGenerator.GenerateMap(width, height, Random.Range(Int32.MinValue, Int32.MaxValue));
        
        SpawnMapTiles(map);
    }

    private void SpawnMapTiles(char[,] tileMap)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject newtile;
                if (tileMap[i, j] == tileData.tiles[0].Identifier)
                {
                    newtile = Instantiate(tileData.tiles[0].Prefab, new Vector3(10 * i, 0, 10 * j), Quaternion.identity, tileParent);
                }
                else
                {
                    newtile = Instantiate(tileData.tiles[1].Prefab, new Vector3(10 * i, 0, 10 * j), Quaternion.identity, tileParent);
                }
            }
        }
    }
}
