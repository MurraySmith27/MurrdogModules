using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckyCoinRelic : Relic
{
    public override bool OnResourcesProcessed(Dictionary<ResourceType, int> totalResourceDiff, 
        Vector2Int position, out Dictionary<ResourceType, int> outResourceDiff, out AdditionalTriggeredArgs args)
    {
        args = new();
        outResourceDiff = new Dictionary<ResourceType, int>();
        if (totalResourceDiff.ContainsKey(ResourceType.Corn) && totalResourceDiff[ResourceType.Corn] > 0)
        {
            PersistentState.Instance.ChangeCurrentGold(1);
            args.LongArg++;
            return true;
        }
        else return false;
    }

    public override void SerializeRelic()
    {
        
    }
}
