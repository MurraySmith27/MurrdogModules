using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoosterPackTypes
{
    NONE,
    BASIC_TILE_BOOSTER,
    ROUND_REWARDS_TILE_BOOSTER,
    ROUND_REWARDS_BUILDING_BOOSTER
}

public class BoosterPackSystem : Singleton<BoosterPackSystem> 
{
    public event Action<BoosterPackTypes> OnBoosterPackOpened;

    public class BoosterPackOfferings
    {
        public List<TileInformation> tiles;
        public List<RelicTypes> relics;
        public List<BuildingType> buildings;
    }

    private BoosterPackTypes _currentBoosterPackType;
    private BoosterPackOfferings _currentOfferings;

    private int _numRefreshes = 0;
    
    public void OpenBoosterPack(BoosterPackTypes type, bool isRefresh = true)
    {
        switch (type)
        {
            case BoosterPackTypes.BASIC_TILE_BOOSTER:
                OpenBasicBoosterPack(GameConstants.NUM_TILES_PER_BASIC_BOOSTER);
                break;
            case BoosterPackTypes.ROUND_REWARDS_TILE_BOOSTER:
                OpenBasicBoosterPack(GameConstants.NUM_TILES_PER_BASIC_BOOSTER);
                break;
            case BoosterPackTypes.ROUND_REWARDS_BUILDING_BOOSTER:
                OpenBuildingsBoosterPack();
                break;
            default:
                break;
        }
        
        _currentBoosterPackType = type;

        if (isRefresh)
        {
            _numRefreshes = 0;
            OnBoosterPackOpened?.Invoke(type);
        }
    }

    private void OpenBasicBoosterPack(int numTiles)
    {
        List<TileInformation> tiles = RandomChanceSystem.Instance.GetTileBoosterPackOfferings(numTiles, _numRefreshes);

        _currentOfferings = new();
        _currentOfferings.tiles = tiles;
    }


    private int num = 0;
    private void OpenBuildingsBoosterPack()
    {
        List<BuildingType> allAvailableBuildingTypes = new List<BuildingType>(GameConstants.STARTING_BUILDING_TYPES);
        allAvailableBuildingTypes.AddRange(TechSystem.Instance.GetUnlockedBuildings());

        if (num == 0)
        {
            allAvailableBuildingTypes = new List<BuildingType>(new BuildingType[]
            {
                BuildingType.Forest, BuildingType.WheatFarm, BuildingType.Mine,
            });
        }
        else if (num == 1)
        {
            allAvailableBuildingTypes = new List<BuildingType>(new BuildingType[]
            {
                BuildingType.LumberMill, BuildingType.CornFarm, BuildingType.CowFarm,
            });
        }
        else
        {
            allAvailableBuildingTypes = new List<BuildingType>(new BuildingType[]
            {   
                BuildingType.Stirrer, BuildingType.Mixer, BuildingType.Mill,
            });
        }

        num++;
        
        List<BuildingType> buildingTypes = RandomChanceSystem.Instance.GetCurrentlyOfferedBuildings(allAvailableBuildingTypes, _numRefreshes);

        _currentOfferings = new();
        _currentOfferings.buildings = buildingTypes;
    }

    public BoosterPackOfferings GetCurrentOfferings()
    {
        return _currentOfferings;
    }

    public void RemoveCurrentOfferings()
    {
        _currentOfferings = null;
    }

    public void RefreshOfferings()
    {
        _numRefreshes++;
        OpenBoosterPack(_currentBoosterPackType, false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            OpenBoosterPack(BoosterPackTypes.ROUND_REWARDS_BUILDING_BOOSTER);
        }
    }
}
