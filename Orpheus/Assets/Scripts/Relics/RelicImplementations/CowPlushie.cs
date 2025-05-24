using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowPlushieRelic : Relic
{
    public override bool OnFoodHarvestedMultCalculated(double multSoFar, ResourceType resourceType,
        out double multDifference, out AdditionalTriggeredArgs args)
    {
        multDifference = 0;
        args = new();
        if (resourceType == ResourceType.Butter || resourceType == ResourceType.Milk)
        {
            multDifference = 1;
            args.LongArg = 1;
            return true;
        }
        else return false;
    }

    public override void SerializeRelic()
    {
        
    }
}
