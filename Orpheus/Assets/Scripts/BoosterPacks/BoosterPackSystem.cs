using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoosterPackTypes
{
    NONE,
    BASIC_TILE_BOOSTER,
    ROUND_REWARDS_TILE_BOOSTER
}

public class BoosterPackSystem : Singleton<BoosterPackSystem> 
{
    public event Action<BoosterPackTypes> OnBoosterPackOpened;

    public class BoosterPackOfferings
    {
        public List<TileInformation> tiles;
        public List<RelicTypes> relics;
    }
    
    private BoosterPackOfferings _currentOfferings;
    
    public void OpenBoosterPack(BoosterPackTypes type)
    {
        switch (type)
        {
            case BoosterPackTypes.BASIC_TILE_BOOSTER:
                OpenBasicBoosterPack(GameConstants.NUM_TILES_PER_BASIC_BOOSTER);
                break;
            case BoosterPackTypes.ROUND_REWARDS_TILE_BOOSTER:
                OpenBasicBoosterPack(GameConstants.NUM_TILES_PER_BASIC_BOOSTER);
                break;
            default:
                break;
        }

        OnBoosterPackOpened?.Invoke(type);
    }

    private void OpenBasicBoosterPack(int numTiles)
    {
        List<TileInformation> tiles = RandomChanceSystem.Instance.GetTileBoosterPackOfferings(numTiles);

        _currentOfferings = new();
        _currentOfferings.tiles = tiles;
    }

    public BoosterPackOfferings GetCurrentOfferings()
    {
        return _currentOfferings;
    }

    public void RemoveCurrentOfferings()
    {
        _currentOfferings = null;
    }
}
