using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TatteredMapRelic : Relic
{
    private const double TATTERED_MAP_RELIC_FOOD_SCORE_PER_TILE = 1d;
    
    public override bool OnFoodScoreConversionComplete(long baseFoodScore, Dictionary<ResourceType, int> resourcesToConvert, out long convertedFoodScore, out AdditionalRelicTriggeredArgs args)
    {
        convertedFoodScore = baseFoodScore;
        args = new();

        List<Guid> cityGuids = MapSystem.Instance.GetAllCityGuids();

        long foodScoreAwarded = (long)Math.Floor((double)MapSystem.Instance.GetAllOwnedCityTiles().Count * TATTERED_MAP_RELIC_FOOD_SCORE_PER_TILE);
        
        convertedFoodScore += foodScoreAwarded;
        
        args.LongArg = foodScoreAwarded;
        
        return foodScoreAwarded > 0;
    }
    
    public override void SerializeRelic()
    {
        
    }
}
