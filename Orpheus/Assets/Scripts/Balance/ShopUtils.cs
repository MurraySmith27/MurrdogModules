using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShopUtils
{
    public static readonly int BONUS_CITIZEN_COST = 100;
    
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
}
