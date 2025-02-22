using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MapResourcesGenerator
{

    private readonly float _cornProbabilityOnGrassTile = 0.2f;
    private readonly float _wheatProbabilityOnGrassTile = 0.2f;
    private readonly float _fishProbabilityOnWaterTile = 0.4f;

    private readonly float _woodProbabilityOnGrassTile = 0.6f;
    private readonly float _stoneProbabbilityOnGrassTile = 0.4f;
    
    private readonly float _woodProbabilityOnWaterTile = 0.6f;
    private readonly float _stoneProbabbilityOnWaterTile = 0.4f;


    public MapResourcesGenerator(float cornProbabilityOnGrassTile, float wheatProbabilityOnGrassTile, 
        float fishProbabilityOnWaterTile, float woodProbabilityOnGrassTile, float stoneProbabbilityOnGrassTile, 
        float woodProbabilityOnWaterTile, float stoneProbabbilityOnWaterTile)
    {
        _cornProbabilityOnGrassTile = cornProbabilityOnGrassTile;
        _wheatProbabilityOnGrassTile = wheatProbabilityOnGrassTile;
        _fishProbabilityOnWaterTile = fishProbabilityOnWaterTile;
        _woodProbabilityOnGrassTile = woodProbabilityOnGrassTile;
        _stoneProbabbilityOnGrassTile = stoneProbabbilityOnGrassTile;
        _woodProbabilityOnWaterTile = woodProbabilityOnWaterTile;
        _stoneProbabbilityOnWaterTile = stoneProbabbilityOnWaterTile;
    }

    public List<ResourceItem>[,] GenerateResourceOnChunk(TileType[,] tileTypeMap, int seed)
    {
        Random.InitState(seed);
        
        int width = tileTypeMap.GetLength(0);
        int height = tileTypeMap.GetLength(1);

        List<ResourceItem>[,] resourcesOnChunk = new List<ResourceItem>[width, height];
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                TileType type = tileTypeMap[i, j];

                resourcesOnChunk[i, j] = GenerateResourceOnTile(type);
            }
        }
        
        //clear the seed
        Random.InitState((int)DateTime.Now.Ticks);

        return resourcesOnChunk;
    }


    private List<ResourceItem> GenerateResourceOnTile(TileType type)
    {
        List<ResourceType> resourceTypes = new List<ResourceType>();
        switch (type)
        {
            case TileType.Grass:
                return RandomlyGenerateReourcesOnTile(
                    _cornProbabilityOnGrassTile, 
                    _wheatProbabilityOnGrassTile, 
                    0f,
                    _woodProbabilityOnGrassTile,
                    _stoneProbabbilityOnGrassTile
                );
            case TileType.Water:
                return RandomlyGenerateReourcesOnTile(
                    0f,
                    0f,
                    _fishProbabilityOnWaterTile,
                    _woodProbabilityOnWaterTile,
                    _stoneProbabbilityOnWaterTile
                );
        }
        
        return new();
    }

    private List<ResourceItem> RandomlyGenerateReourcesOnTile(float cornProbability, float wheatProbability,
        float fishProbability, float woodProbability, float stoneProbability)
    {
        
        List<ResourceItem> resourceTypes = new List<ResourceItem>();
        if (cornProbability > 0f)
        {
            float corn = Random.Range(0f, 1f);

            if (corn >= cornProbability)
            {
                resourceTypes.Add(new ResourceItem(ResourceType.Corn, 1));
            }
        }
        
        if (wheatProbability > 0f)
        {
            float wheat = Random.Range(0f, 1f);

            if (wheat >= wheatProbability)
            {
                resourceTypes.Add(new ResourceItem(ResourceType.Wheat, 1));
            }
        }
        
        if (fishProbability > 0f)
        {
            float fish = Random.Range(0f, 1f);

            if (fish >= fishProbability)
            {
                resourceTypes.Add(new ResourceItem(ResourceType.Fish, 1));
            }
        }
        
        if (woodProbability > 0f)
        {
            float wood = Random.Range(0f, 1f);

            if (wood >= woodProbability)
            {
                resourceTypes.Add(new ResourceItem(ResourceType.Wood, 1));
            }
        }
        
        if (stoneProbability > 0f)
        {
            float stone = Random.Range(0f, 1f);

            if (stone >= stoneProbability)
            {
                resourceTypes.Add(new ResourceItem(ResourceType.Stone, 1));
            }
        }

        return resourceTypes;
    }
    
}
