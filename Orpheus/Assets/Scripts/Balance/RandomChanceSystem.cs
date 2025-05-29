using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomChanceSystem : Singleton<RandomChanceSystem>
{
    [SerializeField] private BuildingProcessRulesSO buildingProcessRules;
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

    private int num = 0;
    public List<RelicTypes> GenerateRelicTypesInShop(int numRelics, int numRefreshes)
    {
        //create a random seed based on the current global seed, the current turn number, and number of refreshes

        num++;
        if (num == 1)
        {
            return new List<RelicTypes>(new RelicTypes[]
            {
                RelicTypes.RUSTY_PLOWSHARE,
                RelicTypes.THE_MOLLUSK,
                RelicTypes.COW_PLUSHIE,
            });
        }
        else if (num == 2)
        {
            return new List<RelicTypes>(new RelicTypes[]
            {
                RelicTypes.BAG_MILK,
                RelicTypes.JELLY_DONUT,
                RelicTypes.THE_MOLLUSK,
            });
        }

        
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

    public List<TileInformation> GetTileBoosterPackOfferings(int numTiles, int numRefreshes)
    {
        int seed = _currentSeed + PersistentState.Instance.HarvestNumber + numRefreshes * 103;
        
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

    public List<BuildingType> GetCurrentlyOfferedBuildings(List<BuildingType> allAvailableBuildings, int numRefreshesUsed)
    {
        if (allAvailableBuildings.Count <= GameConstants.NUM_BUILDINGS_OFFERED_EACH_ROUND)
        {
            return allAvailableBuildings;
        }
        
        int seed = _currentSeed + PersistentState.Instance.HarvestNumber * 340543 + numRefreshesUsed * 31;
        
        Random.InitState(seed);
        
        List<BuildingType> remainingPicks = new List<BuildingType>(allAvailableBuildings);
        
        List<BuildingType> pickedSubset = new();
        
        for (int i = 0; i < GameConstants.NUM_BUILDINGS_OFFERED_EACH_ROUND; i++)
        {
            int newPick = Random.Range(0, remainingPicks.Count);

            BuildingType newBuildingType = remainingPicks[newPick];
            pickedSubset.Add(newBuildingType);
            
            remainingPicks.RemoveAt(newPick);

            if (PersistentState.Instance.HarvestNumber == 0)
            {
                //also offer the corresponding resource generator
                List<PersistentResourceItem> selectedBuildingPersistentInputs =
                    buildingProcessRules.GetPersistentResourceInput(newBuildingType, 0);
                if (selectedBuildingPersistentInputs.FirstOrDefault((persistentResourceItem) =>
                    {
                        return persistentResourceItem.Type == PersistentResourceType.Dirt;
                    }) != null)
                {
                    pickedSubset.Add(BuildingType.DirtPile);
                }
                else
                {
                    pickedSubset.Add(BuildingType.Well);
                }

                break;
            }
        }

        Random.InitState((int)DateTime.Now.Ticks);
        
        return pickedSubset;
    }
}
