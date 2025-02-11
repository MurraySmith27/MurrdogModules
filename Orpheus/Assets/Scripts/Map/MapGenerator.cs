using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator
{
    [Header("Cellular Automata Settings")]
    private readonly float _noiseDensity = 0.5f;
    private readonly int _cellularAutomataIterations = 5;
    private readonly int _numAdjacentCellsToMakeLand = 4;

    private List<TileDescriptor> _tileDescriptors;
    
    public MapGenerator(float noiseDensity, int cellularAutomataIterations, int numAdjacentCellsToMakeLand, List<TileDescriptor> tileDescriptors)
    {
        this._noiseDensity = noiseDensity;
        this._cellularAutomataIterations = cellularAutomataIterations;
        this._numAdjacentCellsToMakeLand = numAdjacentCellsToMakeLand;
        this._tileDescriptors = tileDescriptors;
    }
    
    private void Start()
    {
        GenerateMap(100, 100, Random.Range(Int32.MinValue, Int32.MaxValue));
    }

    public char[,] GenerateMap(int width, int height, int seed)
    {
        Random.InitState(seed);
        
        //first we generate a land/water map using cellular automata.
        
        //create a noise map
        bool[,] a = GenerateNoiseGrid(width, height, _noiseDensity);
        bool[,] b = new bool[width, height];

        for (int i = 0; i < _cellularAutomataIterations; i++)
        {
            //alternate to avoid reallocating each iteration.
            if (i % 2 == 0)
            {
                RunCellularAutomata(a, ref b);
            }
            else
            {
                RunCellularAutomata(b, ref a);
            }
        }

        bool[,] generatedMap;
        if (_cellularAutomataIterations % 2 == 0)
        {
            generatedMap = a;
        }
        else
        {
            generatedMap = b;
        }
        
        char[,] mapTiles = new char[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (generatedMap[i, j])
                {
                    mapTiles[i, j] = _tileDescriptors[0].Identifier;
                }
                else
                {
                    mapTiles[i, j] = _tileDescriptors[1].Identifier;
                }
            }
        }

        return mapTiles;
    }

    private void RunCellularAutomata(bool[,] inputMap, ref bool[,] outputMap)
    {
        int width = inputMap.GetLength(0);
        int height = inputMap.GetLength(1);

        if (outputMap == null || outputMap.GetLength(0) != width || outputMap.GetLength(1) != height)
        {
            outputMap = new bool[width, height];
        }
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int landCount = 0;
                bool isLand = false;
                for (int x = i - 1; x <= i + 1; x++)
                {
                    for (int y = j - 1; y <= j + 1; y++)
                    {
                        bool land;
                        if (x < 0 || x >= width || y < 0 || y >= height)
                        {
                            land = Random.Range(0, 1) > 0.5f;
                        }
                        else
                        {
                            land = inputMap[x, y];
                        }
                        
                        if (land)
                        {
                            landCount++;
                        }

                        if (landCount >= _numAdjacentCellsToMakeLand)
                        {
                            isLand = true;
                            break;
                        }
                    }
                    
                    if (isLand)
                    {
                        break;
                    }
                }

                outputMap[i, j] = isLand;
            }
        }
        
        //clear the seed
        Random.InitState((int)DateTime.Now.Ticks);
    }

    private bool[,] GenerateNoiseGrid(int width, int height, float noiseDensity)
    {
        noiseDensity = Mathf.Clamp01(noiseDensity);
        
        //row-first indexing
        bool[,] noiseGrid = new bool[width, height];
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (Random.Range(0f, 1f) > noiseDensity)
                {
                    noiseGrid[i, j] = false;
                }
                else
                {
                    noiseGrid[i, j] = true;
                }
            }
        }

        return noiseGrid;
    }
}
