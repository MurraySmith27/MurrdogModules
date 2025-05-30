using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TatteredMapRelic : Relic
{
    public override bool OnResourcesProcessed(Dictionary<ResourceType, int> totalResourceDiff, 
        Vector2Int position, out Dictionary<ResourceType, int> outResourceDiff, 
        out Dictionary<PersistentResourceType, int> outPersistentResourcesDiff, out AdditionalTriggeredArgs args)
    {
        args = new();
        outResourceDiff = new Dictionary<ResourceType, int>();
        outPersistentResourcesDiff = new();
        if ((totalResourceDiff.ContainsKey(ResourceType.Flour) && totalResourceDiff[ResourceType.Flour] > 0) || 
            (totalResourceDiff.ContainsKey(ResourceType.Dough) && totalResourceDiff[ResourceType.Dough] > 0) ||
            (totalResourceDiff.ContainsKey(ResourceType.Bread) && totalResourceDiff[ResourceType.Bread] > 0) ||
            (totalResourceDiff.ContainsKey(ResourceType.Toast) && totalResourceDiff[ResourceType.Toast] > 0) ||
            (totalResourceDiff.ContainsKey(ResourceType.ButteredToast) && totalResourceDiff[ResourceType.ButteredToast] > 0))
        {
            outPersistentResourcesDiff.Add(PersistentResourceType.Gold, 1);
            args.LongArg++;
            return true;
        }
        else return false;
    }
    
    public override void SerializeRelic()
    {
        
    }
}
