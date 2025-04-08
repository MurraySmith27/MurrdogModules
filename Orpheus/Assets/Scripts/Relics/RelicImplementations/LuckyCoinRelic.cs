using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckyCoinRelic : Relic
{
    private const long LUCKY_COIN_INTEREST_CAP_INCREASE = 50;
    
    public override bool OnGoldInterestAdded(long coinTotalBefore, long interest, out long newInterest, out AdditionalTriggeredArgs args)
    {
        newInterest = interest;
        args = new();
        
        
        if (coinTotalBefore >= GameConstants.GOLD_INTEREST_CAP)
        {
            long additionalInterest = ((long)Mathf.Min(coinTotalBefore - GameConstants.GOLD_INTEREST_CAP, LUCKY_COIN_INTEREST_CAP_INCREASE) % GameConstants.GOLD_INTEREST_INTERVAL) * GameConstants.GOLD_INTEREST_PER_INTERVAL;
            
            newInterest += additionalInterest;
            
            args.LongArg = additionalInterest;
            
            return true;
        }
        
        return false;
    }

    public override void SerializeRelic()
    {
        
    }
}
