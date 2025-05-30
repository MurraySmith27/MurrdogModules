using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaptainsHookRelic : Relic
{
    public override bool OnFoodHarvestedMultCalculated(double multSoFar, ResourceType resourceType,
        out double multDifference, out AdditionalTriggeredArgs args)
    {
        multDifference = 0;
        args = new();
        if (resourceType == ResourceType.Sushi)
        {
            multDifference = multSoFar;
            args.LongArg = (long)multSoFar;
            return true;
        }
        else return false;
    }
    
    public override void SerializeRelic()
    {
        
    }
}
