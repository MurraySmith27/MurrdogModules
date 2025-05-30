using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RustyPlowshareRelic : Relic
{
    public override bool OnResourcesProcessed(Dictionary<ResourceType, int> totalResourceDiff, 
        Vector2Int position, out Dictionary<ResourceType, int> outResourceDiff, 
        out Dictionary<PersistentResourceType, int> outPersistentResourcesDiff, out AdditionalTriggeredArgs args)
    {
        args = new();
        outResourceDiff = new Dictionary<ResourceType, int>();
        outPersistentResourcesDiff = new();
        if (totalResourceDiff.ContainsKey(ResourceType.Wheat) && totalResourceDiff[ResourceType.Wheat] > 0)
        {
            if (!outResourceDiff.ContainsKey(ResourceType.Corn))
            {
                outResourceDiff.Add(ResourceType.Corn, 0);
            }

            outResourceDiff[ResourceType.Corn]++;
            args.LongArg++;
            return true;
        }
        else return false;
    }

    public override void SerializeRelic()
    {
        
    }
}
