using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneRoseRelic : Relic
{
    public override bool OnResourcesHarvested(Dictionary<ResourceType, int> resourcesOnTile, Dictionary<ResourceType, int> resourcesToBeHarvested, Vector2Int position, out Dictionary<ResourceType, int> outResourcesOnTile, out  Dictionary<ResourceType, int> outResourcesToBeHarvested, out AdditionalTriggeredArgs args)
    {
        outResourcesOnTile = resourcesOnTile;
        outResourcesToBeHarvested = resourcesToBeHarvested;
        args = new();

        if (resourcesOnTile.ContainsKey(ResourceType.Stone))
        {
            if (!outResourcesOnTile.ContainsKey(ResourceType.Corn))
            {
                outResourcesOnTile.Add(ResourceType.Corn, 0);
            }

            outResourcesOnTile[ResourceType.Corn] += 1;
            
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
