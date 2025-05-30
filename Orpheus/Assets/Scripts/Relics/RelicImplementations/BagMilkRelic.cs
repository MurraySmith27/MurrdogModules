using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagMilkRelic : Relic
{
    public override bool OnResourcesProcessed(Dictionary<ResourceType, int> totalResourceDiff, 
        Vector2Int position, out Dictionary<ResourceType, int> outResourceDiff, 
        out Dictionary<PersistentResourceType, int> outPersistentResourcesDiff, out AdditionalTriggeredArgs args)
    {
        args = new();
        outResourceDiff = new Dictionary<ResourceType, int>();
        outPersistentResourcesDiff = new();
        if ((totalResourceDiff.ContainsKey(ResourceType.Milk) && totalResourceDiff[ResourceType.Milk] > 0) || (totalResourceDiff.ContainsKey(ResourceType.Butter) && totalResourceDiff[ResourceType.Butter] > 0))
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
