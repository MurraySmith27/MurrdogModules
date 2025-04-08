using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory
{
    public Item CreateItem(ItemTypes type)
    {
        switch (type)
        {
            case ItemTypes.NONE:
                return null;
            case ItemTypes.BONUS_CITIZEN:
                return new BonusCitizenItem();
        }

        return null;
    }
}
