using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Cellular Automata Settings")]
    [SerializeField] private float noiseDensity = 0.5f;
    [SerializeField] private int cellularAutomataIterations = 5;
    [SerializeField] private int numAdjacentCellsToMakeLand = 4;

    [SerializeField] private List<GameObject> TEMPPREFABS;
    [SerializeField] private Transform tileParent;
    // [SerializeField] private List<MapTileDescriptor> mapTileDescriptors;

    private void Start()
    {
        GenerateMap(100, 100);
    }
    
    public int[,] GenerateMap(int width, int height)
    {
        //first we generate a land/water map using cellular automata.
        
        //create a noise

        bool[,] a = GenerateNoiseGrid(width, height, noiseDensity);
        bool[,] b = new bool[width, height];

        for (int i = 0; i < cellularAutomataIterations; i++)
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
        if (cellularAutomataIterations % 2 == 0)
        {
            generatedMap = a;
        }
        else
        {
            generatedMap = b;
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject newtile;
                if (generatedMap[i, j])
                {
                    newtile = Instantiate(TEMPPREFABS[0], new Vector3(10 * i, 0, 10 * j), Quaternion.identity, tileParent);
                }
                else
                {
                    newtile = Instantiate(TEMPPREFABS[1], new Vector3(10 * i, 0, 10 * j), Quaternion.identity, tileParent);
                }
            }
        }

        return new int[,]{};
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

                        if (landCount >= numAdjacentCellsToMakeLand)
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
