using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaptainsHookRelic : Relic
{
    public override bool OnResourcesHarvested(Dictionary<ResourceType, int> resourcesOnTile, Dictionary<ResourceType, int> resourcesToBeHarvested, Vector2Int position, out Dictionary<ResourceType, int> outResourcesOnTile, out Dictionary<ResourceType, int> outResourcesToBeHarvested, out AdditionalTriggeredArgs args)
    {
        args = new();

        outResourcesToBeHarvested = resourcesToBeHarvested;
        outResourcesOnTile = resourcesOnTile;
        
        TileType tileType = MapSystem.Instance.GetTileType(position.x, position.y);

        if (tileType == TileType.Water && resourcesOnTile.ContainsKey(ResourceType.Wood))
        {
            if (!outResourcesToBeHarvested.ContainsKey(ResourceType.Wood))
            {
                outResourcesToBeHarvested.Add(ResourceType.Wood, 0);
            }

            resourcesOnTile[ResourceType.Wood] += 1;
            outResourcesToBeHarvested[ResourceType.Wood] += 1;
            return true;
        }
        
        return false;
    }
    
    public override void SerializeRelic()
    {
        
    }
}
