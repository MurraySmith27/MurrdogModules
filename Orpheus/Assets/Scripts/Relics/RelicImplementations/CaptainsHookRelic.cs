using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptainsHookRelic : Relic
{
    public override bool OnResourcesHarvested(Dictionary<ResourceType, int> resourcesToBeHarvested, Vector2Int position, out  Dictionary<ResourceType, int> outResourcesToBeHarvested, out AdditionalTriggeredArgs args)
    {
        args = new();

        outResourcesToBeHarvested = resourcesToBeHarvested;
        
        TileType tileType = MapSystem.Instance.GetTileType(position.x, position.y);

        if (tileType == TileType.Water)
        {
            if (!outResourcesToBeHarvested.ContainsKey(ResourceType.Wood))
            {
                outResourcesToBeHarvested.Add(ResourceType.Wood, 0);
            }

            outResourcesToBeHarvested[ResourceType.Wood] += 1;
            return true;
        }
        
        return false;
    }
    
    public override void SerializeRelic()
    {
        
    }
}
