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
                return new PrivateEyesRelic();
                break;
            case RelicTypes.RUSTY_PLOWSHARE:
                return new RustyPlowshareRelic();
                break;
            case RelicTypes.CAPTAINS_HOOK:
                return new CaptainsHookRelic();
                break;
            case RelicTypes.BAKERS_DOZEN:
                return new BakersDozenRelic();
                break;
            case RelicTypes.LUCKY_COIN:
                return new LuckyCoinRelic();
                break;
            case RelicTypes.BUSINESS_CARD:
                return new BusinessCardRelic();
                break;
            case RelicTypes.JELLY_DONUT:
                return new JellyDonutRelic();
                break;
            case RelicTypes.TATTERED_MAP:
                return new TatteredMapRelic();
                break;
            case RelicTypes.THE_MOLLUSK:
                return new TheMolluskRelic();
                break;
            case RelicTypes.STONE_ROSE:
                return new StoneRoseRelic();
                break;
            case RelicTypes.EXTRA_HAND:
                return new ExtraHandRelic();
                break;
            case RelicTypes.EXTRA_DISCARD:
                return new ExtraDiscardRelic();
                break;
            case RelicTypes.COW_PLUSHIE:
                return new CowPlushieRelic();
                break;
            case RelicTypes.BAG_MILK:
                return new BagMilkRelic();
                break;
        }

        return null;
    }
}
