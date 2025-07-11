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
        1,
        2,
        3,
        5,   
        7,   
        9,   
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
    
    public static readonly long STARTING_STONE = 1;

    public static readonly long STARTING_BUILD_TOKENS = 100;

    public static readonly long STARTING_GOLD = 42;

    public static readonly long GOLD_PER_HARVEST = 100;

    public static readonly long HARVEST_GOLD_PER_UNUSED_HAND = 30;
    
    public static readonly Vector2Int[] INITIAL_CITY_TILES = new Vector2Int[]
    {
        new Vector2Int(0, 0), new Vector2Int(-1, 0),
        new Vector2Int(1, -1), new Vector2Int(-1, -1),
        new Vector2Int(1, -2), new Vector2Int(0, -2),
    };
    
    public static readonly TileType[] INITIAL_CITY_TILE_TYPES = new TileType[]
    {
        TileType.Grass, TileType.Grass,
        TileType.Grass, TileType.Grass,
        TileType.Grass, TileType.Grass,
    };

    public static readonly BuildingType[] INITIAL_CITY_BUILDINGS = new BuildingType[]
    {
        BuildingType.CityCapital, BuildingType.CityCapital,
        BuildingType.CityCapital, BuildingType.CityCapital,
        BuildingType.CityCapital, BuildingType.CityCapital,
    };
    
    public static readonly ResourceItem[] INITIAL_CITY_RESOURCES = new ResourceItem[]
    {
        new ResourceItem(ResourceType.Wood, 0), new ResourceItem(ResourceType.Wood, 0),
        new ResourceItem(ResourceType.Wood, 0), new ResourceItem(ResourceType.Wood, 0),
        new ResourceItem(ResourceType.Wood, 0), new ResourceItem(ResourceType.Wood, 0),
        
    };
    
    //interest
    public static readonly long GOLD_INTEREST_CAP = 5000;
    public static readonly long GOLD_INTEREST_INTERVAL = 500;
    public static readonly long GOLD_INTEREST_PER_INTERVAL = 0;//100;
    
    // building yields

    public static readonly int CORN_PER_CORN_FARM = 1;
    public static readonly int WHEAT_PER_WHEAT_FARM = 1;
    public static readonly int FISH_PER_FISH_FARM = 1;
    public static readonly int WHEAT_PER_BREAD = 1;
    public static readonly int BREAD_PER_BAKERY = 1;
    public static readonly int WOOD_PER_LUMBER = 1;
    public static readonly int LUMBER_PER_LUMBER_MILL = 1;
    public static readonly int STONE_PER_COPPER = 1;
    public static readonly int COPPER_PER_COOPPER_YARD = 1;
    public static readonly int COPPER_PER_STEEL = 1;
    public static readonly int STEEL_PER_STEEL_YARD = 1;
    public static readonly int CORN_PER_POPCORN = 1;
    public static readonly int POPCORN_PER_POPCORN_FACTORY = 1;
    public static readonly int FISH_PER_SUSHI = 1;
    public static readonly int SUSHI_PER_SUSHI_RESTAURANT = 1;


    public static readonly int NUM_BUILDINGS_OFFERED_EACH_ROUND = 3;
    public static readonly int NUM_FREE_BUILDING_REFRESHES = 1;
    public static readonly long INITIAL_BUILDING_REFRESH_GOLD_COST = 20;
    public static readonly double GOLD_COST_PER_BUILDING_REFRESH_MULTIPLIER = 1.5d;

    public static readonly Dictionary<ResourceType, double> BASE_FOOD_SCORE_PER_RESOURCE =
    new Dictionary<ResourceType, double> {
        {ResourceType.Corn, 1d},
        {ResourceType.Wheat, 1d},
        {ResourceType.Bread, 3d},
        {ResourceType.Fish, 2d},
        {ResourceType.Wood, 1d},
        {ResourceType.Stone, 1d},
        {ResourceType.Lumber, 3d},
        {ResourceType.Copper, 3d},
        {ResourceType.Steel, 9d},
        {ResourceType.Popcorn, 4d},
        {ResourceType.Sushi, 5d},
        {ResourceType.Flour, 3d},
        {ResourceType.Dough, 5d},
        {ResourceType.Toast, 12d},
        {ResourceType.ButteredToast, 25d},
        {ResourceType.Butter, 7d},
        {ResourceType.Milk, 4d},
    };

    public static readonly long GOLD_PER_LEFTOVER_FOOD_SCORE = 10;
    
    
    //shop settings
    public static readonly long SHOP_REFRESH_GOLD_INITIAL_COST = 3;
    public static readonly long SHOP_REFRESH_GOLD_INCREASE = 1;

    public static readonly long SHOP_RELIC_COST = 5;
    

    public static readonly float SELL_VALUE_PERCENTAGE = 0.4f;
    
    //booster pack settings
    public static readonly int NUM_TILES_PER_BASIC_BOOSTER = 3; 
    
    
    //item settings
    public static readonly int STARTING_ITEM_CAPACITY = 2;
    
    //harvest settings
    public static readonly int STARTING_CITIZENS_PER_HARVEST_ROUND = 3;
    public static readonly int STARTING_DISCARDS_PER_HARVEST_ROUND = 3;
    public static readonly int STARTING_HANDS_PER_HARVEST_ROUND = 3;

    public static readonly List<BuildingType> STARTING_BUILDING_TYPES = new List<BuildingType>(
        new BuildingType[]
        {
            BuildingType.Forest,
            BuildingType.Mine,
            BuildingType.WheatFarm,
            BuildingType.CornFarm
        }
        );
}
