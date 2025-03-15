using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyDonutRelic : Relic
{
    private const int JELLY_DONUT_RELIC_GOLD_PER_WHEAT = 10;
    
    public override bool OnFoodScoreConversion(long baseFoodScore, Dictionary<ResourceType, int> resourcesToConvert, out long convertedFoodScore, out AdditionalRelicTriggeredArgs args)
    {
        convertedFoodScore = baseFoodScore;
        args = new();

        int amountOfWheat = 0;
        if (resourcesToConvert.ContainsKey(ResourceType.Wheat))
        {
            amountOfWheat = resourcesToConvert[ResourceType.Wheat];
        }

        int goldAwarded = JELLY_DONUT_RELIC_GOLD_PER_WHEAT * amountOfWheat;

        if (goldAwarded > 0)
        {
            PlayerResourcesSystem.Instance.AddResource(PersistentResourceType.Gold, goldAwarded);
        }

        args.IntArg = goldAwarded;
        
        return amountOfWheat > 0;
    }

    public override void SerializeRelic()
    {
        
    }
}
