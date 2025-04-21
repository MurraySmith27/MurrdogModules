using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusinessCardRelic : Relic
{
    private const double BUSINESS_CARD_RELIC_GOLD_TO_FOOD_SCORE_RATE = 0.1f;

    private const int BUSINESS_CARD_FOOD_SCORE_PER_GOLD = 1;
    
    public override bool OnFoodScoreConversionComplete(long baseFoodScore, Dictionary<ResourceType, int> resourcesToConvert, out long convertedFoodScore, out AdditionalTriggeredArgs args)
    {
        convertedFoodScore = baseFoodScore;
        args = new();
        
        long removedGold = (long)Math.Floor(PersistentState.Instance.CurrentGold * BUSINESS_CARD_RELIC_GOLD_TO_FOOD_SCORE_RATE);
        
        long addedFoodScore = removedGold * BUSINESS_CARD_FOOD_SCORE_PER_GOLD;
        
        PersistentState.Instance.ChangeCurrentGold(-removedGold);
        
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
