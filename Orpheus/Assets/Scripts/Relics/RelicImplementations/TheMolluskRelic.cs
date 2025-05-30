using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheMolluskRelic : Relic
{
    public override bool OnResourcesProcessed(Dictionary<ResourceType, int> totalResourceDiff, 
        Vector2Int position, out Dictionary<ResourceType, int> outResourceDiff, 
        out Dictionary<PersistentResourceType, int> outPersistentResourcesDiff, out AdditionalTriggeredArgs args)
    {
        args = new();
        outResourceDiff = new Dictionary<ResourceType, int>();
        outPersistentResourcesDiff = new();
        if (totalResourceDiff.ContainsKey(ResourceType.Fish) && totalResourceDiff[ResourceType.Fish] > 0)
        {
            outResourceDiff.Add(ResourceType.Fish, 1);
            args.LongArg++;
            return true;
        }
        else return false;
    }

    public override void SerializeRelic()
    {
        
    }
}
