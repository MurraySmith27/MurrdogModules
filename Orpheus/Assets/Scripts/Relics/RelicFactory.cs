using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicFactory
{
    public Relic CreateRelic(RelicTypes type)
    {
        switch (type)
        {
            case RelicTypes.NONE:
                return null;
            case RelicTypes.PRIVATE_EYES:
                break;
            case RelicTypes.RUSTY_PLOWSHARE:
                break;
            case RelicTypes.CAPTAINS_HOOK:
                break;
            case RelicTypes.BAKERS_DOZEN:
                break;
            case RelicTypes.LUCKY_COIN:
                break;
            case RelicTypes.BUSINESS_CARD:
                break;
            case RelicTypes.JELLY_DONUT:
                break;
            case RelicTypes.TATTERED_MAP:
                break;
            case RelicTypes.THE_MOLLUSK:
                break;
            case RelicTypes.STONE_ROSE:
                break;
        }

        return null;
    }
}
