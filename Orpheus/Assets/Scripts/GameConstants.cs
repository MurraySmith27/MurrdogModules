using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public static readonly int TILE_SIZE = 10;

    public static readonly long[] FOOD_GOALS_PER_ROUND =
    {
        3,
        8,
        28,
        60,
        110,
        200,
        350,
        560,
        7200,
        300000,
        47000000000,
        29000000000000,
        77000000000000000,
    };

    public static readonly long STARTING_WOOD = 100;

    public static readonly long STARTING_STONE = 100;

    public static readonly long STARTING_GOLD = 1000;

    public static readonly Vector2Int[] INITIAL_CITY_TILES = new Vector2Int[]
    {
        new Vector2Int(0,2),
        new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
        new Vector2Int(-2, 0), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0),
        new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1),
        new Vector2Int(0, -2)
    };
    
    //interest
    public static readonly long GOLD_INTEREST_CAP = 50;
    public static readonly long GOLD_INTEREST_INTERVAL = 10;
    public static readonly long GOLD_INTEREST_PER_INTERVAL = 1;
    
    // building yields

    public static readonly int CORN_PER_CORN_FARM = 1;
    public static readonly int WHEAT_PER_WHEAT_FARM = 1;
    public static readonly int FISH_PER_FISH_FARM = 1;
    public static readonly int WHEAT_PER_BREAD = 1;
    public static readonly int BREAD_PER_BAKERY = 1;

}
