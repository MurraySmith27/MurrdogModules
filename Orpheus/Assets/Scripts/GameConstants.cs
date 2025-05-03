using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public static readonly int TILE_SIZE = 10;

    public static readonly float HEX_SIDE_LENGTH = 0.5f;

    public static readonly Vector2Int STARTING_MAP_SIZE = new Vector2Int(50, 50);

    public static readonly long[] FOOD_GOALS_PER_HARVEST =
    {
        2,
        4,
        6,
        8,
        12,
        16,
        20,
        32,
        40,
        50,
        70,
        90,
        110,
        140,
        180,
        200,
        250,
        300,
        350,
        420,
        470,
        500
    };

    public static readonly long STARTING_WOOD = 1;

    public static readonly long STARTING_LUMBER = 0;
    
    public static readonly long STARTING_STONE = 1;
    
    public static readonly long STARTING_COPPER = 0;
    
    public static readonly long STARTING_STEEL = 0;

    public static readonly long STARTING_GOLD = 400;

    public static readonly long GOLD_PER_HARVEST = 100;

    public static readonly long HARVEST_GOLD_PER_UNUSED_HAND = 30;
    
    public static readonly Vector2Int[] INITIAL_CITY_TILES = new Vector2Int[]
    {
        new Vector2Int(1, 0), new Vector2Int(1, -1),
        new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(0, -1),
        new Vector2Int(-1, 1), new Vector2Int(-1, 0)
    };
    
    public static readonly TileType[] INITIAL_CITY_TILE_TYPES = new TileType[]
    {
        TileType.Grass, TileType.Grass,
        TileType.Grass, TileType.Grass, TileType.Grass,
        TileType.Water, TileType.Water,
    };
    
    public static readonly ResourceItem[] INITIAL_CITY_RESOURCES = new ResourceItem[]
    {
        new ResourceItem(ResourceType.Corn, 1), new ResourceItem(ResourceType.Stone, 1), 
        new ResourceItem(ResourceType.Wheat, 0), new ResourceItem(ResourceType.Wood, 0), new ResourceItem(ResourceType.Wood, 1),
        new ResourceItem(ResourceType.Stone, 0), new ResourceItem(ResourceType.Wood, 0)
    };
    
    
    //interest
    public static readonly long GOLD_INTEREST_CAP = 5000;
    public static readonly long GOLD_INTEREST_INTERVAL = 500;
    public static readonly long GOLD_INTEREST_PER_INTERVAL = 100;
    
    // building yields

    public static readonly int CORN_PER_CORN_FARM = 1;
    public static readonly int WHEAT_PER_WHEAT_FARM = 1;
    public static readonly int FISH_PER_FISH_FARM = 1;
    public static readonly int WHEAT_PER_BREAD = 1;
    public static readonly int BREAD_PER_BAKERY = 1;

    public static readonly Dictionary<ResourceType, double> BASE_FOOD_SCORE_PER_RESOURCE =
    new Dictionary<ResourceType, double> {
        {ResourceType.Corn, 1d},
        {ResourceType.Wheat, 0d},
        {ResourceType.Bread, 3d},
        {ResourceType.Fish, 2d}
    };

    public static readonly long GOLD_PER_LEFTOVER_FOOD_SCORE = 10;
    
    
    //shop settings
    public static readonly long SHOP_REFRESH_GOLD_INITIAL_COST = 100;
    public static readonly long SHOP_REFRESH_GOLD_INCREASE = 50;

    public static readonly long SHOP_RELIC_COST = 500;
    

    public static readonly float SELL_VALUE_PERCENTAGE = 0.4f;
    
    //booster pack settings
    public static readonly int NUM_TILES_PER_BASIC_BOOSTER = 3; 
    
    
    //item settings
    public static readonly int STARTING_ITEM_CAPACITY = 2;
    
    //harvest settings
    public static readonly int STARTING_CITIZENS_PER_HARVEST_ROUND = 3;
    public static readonly int STARTING_DISCARDS_PER_HARVEST_ROUND = 3;
    public static readonly int STARTING_HANDS_PER_HARVEST_ROUND = 3;
}
