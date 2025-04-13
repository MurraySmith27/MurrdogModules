using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShopUtils
{
    //Item costs
    public static readonly int BONUS_CITIZEN_COST = 100;
    
    //Booster Pack Costs
    public static readonly int BASIC_TILE_BOOSTER_COST = 100;
    
    public static long GetCostOfItem(ItemTypes itemTypes)
    {
        switch (itemTypes)
        {
            case ItemTypes.BONUS_CITIZEN:
                return BONUS_CITIZEN_COST;
            default:
                return 0;
        }
    }

    public static long GetCostOfBoosterPack(BoosterPackTypes boosterPackType)
    {
        switch (boosterPackType)
        {
            case BoosterPackTypes.BASIC_TILE_BOOSTER:
                return BASIC_TILE_BOOSTER_COST;
            default:
                return 0;
        }
    }
}
