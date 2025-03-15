using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneRoseRelic : Relic
{
    public override bool OnResourcesHarvested(Dictionary<ResourceType, int> resourcesToBeHarvested, Vector2Int position, out  Dictionary<ResourceType, int> outResourcesToBeHarvested, out AdditionalRelicTriggeredArgs args)
    {
        outResourcesToBeHarvested = resourcesToBeHarvested;
        args = new();

        if (outResourcesToBeHarvested.ContainsKey(ResourceType.Stone))
        {
            if (!outResourcesToBeHarvested.ContainsKey(ResourceType.Corn))
            {
                outResourcesToBeHarvested.Add(ResourceType.Corn, 0);
            }
            
            outResourcesToBeHarvested[ResourceType.Corn] += 1;

            return true;
        }
        
        return false;
    }

    public override void SerializeRelic()
    {
        
    }
}
