using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomChanceSystem : Singleton<RandomChanceSystem>
{

    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int seed = 0;

    private int _currentSeed;

    private void Awake()
    {
        //TODO: Temp, make an actual game start thing.
        OnGameStart();
    }

    private void Start()
    {
        GameStartController.Instance.OnGameStart -= OnGameStart;
        GameStartController.Instance.OnGameStart += OnGameStart;
    }

    private void OnDestroy()
    {
        if (GameStartController.IsAvailable)
        {
            GameStartController.Instance.OnGameStart -= OnGameStart;
        }
    }

    private void OnGameStart()
    {
        _currentSeed = seed;
        if (useRandomSeed)
        {
            _currentSeed = UnityEngine.Random.Range(Int32.MinValue, Int32.MaxValue);
        }
    }

    public int GetCurrentSeed()
    {
        return _currentSeed;
    }

    public List<RelicTypes> GenerateRelicTypesInShop(int numRelics, int numRefreshes)
    {
        //create a random seed based on the current global seed, the current turn number, and number of refreshes

        int seed = _currentSeed + 3605 * PersistentState.Instance.RoundNumber + 2821 *  numRefreshes;
        
        Random.InitState(seed);
        
        List<RelicTypes> unownedRelicTypes = new List<RelicTypes>();

        List<RelicTypes> ownedRelicTypes = RelicSystem.Instance.GetOwnedRelics();
        for (int i = 1; i < Enum.GetValues(typeof(RelicTypes)).Length; i++)
        {
            RelicTypes relicType = (RelicTypes)i;
            if (!ownedRelicTypes.Contains(relicType))
            {
                unownedRelicTypes.Add(relicType);
            }
        }

        List<RelicTypes> selectedRelics = new();

        int maxRelicValue = Enum.GetValues(typeof(RelicTypes)).Length;

        for (int i = 0; i < numRelics; i++)
        {
            int newRelic = Random.Range(1, maxRelicValue);
            bool foundNewRelic = false;

            if (selectedRelics.Contains((RelicTypes)newRelic) || ownedRelicTypes.Contains((RelicTypes)newRelic))
            {
                for (int j = 0; j < maxRelicValue - 1; j++)
                {
                    int tryRelic = (newRelic + j - 1) % (maxRelicValue - 1) + 1;
                    if (!selectedRelics.Contains((RelicTypes)tryRelic) &&
                        !ownedRelicTypes.Contains((RelicTypes)tryRelic))
                    {
                        foundNewRelic = true;
                        newRelic = tryRelic;
                        break;
                    }
                }
            }
            else foundNewRelic = true;

            if (foundNewRelic)
            {
                selectedRelics.Add((RelicTypes)newRelic);
            }
        }
        
        //reset the seed
        Random.InitState((int)DateTime.Now.Ticks);

        return selectedRelics;
    }

    public Vector2Int GetNextCitizenTile(List<Vector2Int> possibleTiles)
    {
        int seed = _currentSeed + 3605 * PersistentState.Instance.HarvestNumber + 2821 * HarvestState.Instance.NumHandsUsed + 31 * HarvestState.Instance.NumDiscardsUsed + 7471 * HarvestState.Instance.NumCitizensUsedThisHarvest;
        Random.InitState(seed);
        
        Vector2Int tile = possibleTiles[Random.Range(0, possibleTiles.Count)];
        
        Random.InitState((int)DateTime.Now.Ticks);
        
        return tile;
    }

    public List<TileInformation> GetTileBoosterPackOfferings(int numTiles)
    {
        int seed = _currentSeed + PersistentState.Instance.HarvestNumber;
        
        Random.InitState(seed);

        List<TileInformation> tiles = new();

        for (int i = 0; i < numTiles; i++)
        {
            TileInformation newTile = new();

            newTile.Type = (TileType)Random.Range(0, Enum.GetNames(typeof(TileType)).Length - 1) + 1;

            // newTile.Resources = MapSystem.Instance.GenerateResourcesOnTile(newTile.Type);
            newTile.Resources = new();
            
            // int randomBuildingType = Random.Range(1, Enum.GetNames(typeof(BuildingType)).Length);
            
            newTile.Buildings = new();
            
            // if (BuildingsController.Instance.CanConstructBuildingOnTileType(newTile.Type,
            //         (BuildingType)randomBuildingType))
            // {
            //     TileBuilding building = new TileBuilding();
            //     
            //     building.Type = (BuildingType)randomBuildingType;
            //     
            //     newTile.Buildings.Add(building);
            // }
            
            tiles.Add(newTile);
        }
        
        Random.InitState((int)DateTime.Now.Ticks);

        return tiles;
    }
}
