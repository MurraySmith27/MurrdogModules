using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusinessCardRelic : Relic
{
    private const double BUSINESS_CARD_RELIC_GOLD_TO_FOOD_SCORE_RATE = 0.1f;

    private const int BUSINESS_CARD_FOOD_SCORE_PER_GOLD = 1;
    
    public override bool OnFoodScoreConversion(long baseFoodScore, Dictionary<ResourceType, int> resourcesToConvert, out long convertedFoodScore, out AdditionalRelicTriggeredArgs args)
    {
        convertedFoodScore = baseFoodScore;
        args = new();
        
        long removedGold = (long)Math.Floor(RoundState.Instance.CurrentGold * BUSINESS_CARD_RELIC_GOLD_TO_FOOD_SCORE_RATE);
        
        long addedFoodScore = removedGold * BUSINESS_CARD_FOOD_SCORE_PER_GOLD;
        
        convertedFoodScore += addedFoodScore;
        
        args.LongListArgs = new();
        args.LongListArgs.Add(removedGold);
        args.LongListArgs.Add(addedFoodScore);
        
        return addedFoodScore > 0;
    }

    public override void SerializeRelic()
    {
        
    }
}
