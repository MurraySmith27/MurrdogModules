using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class BakersDozenRelic : Relic
{
    private const float BAKERS_DOZEN_RELIC_PROBABILITY = 0.5f;
    
    public override bool OnResourcesProcessed(Dictionary<ResourceType, int> resourceDiff, Vector2Int position,
        out Dictionary<ResourceType, int> outResourceDiff, out AdditionalRelicTriggeredArgs args)
    {
        args = new();

        outResourceDiff = resourceDiff;

        List<TileBuilding> buildingTiles = MapSystem.Instance.GetBuildingsOnTile(position);

        bool containsBakery = false;
        
        foreach (TileBuilding building in buildingTiles) {

            if (building.Type == BuildingType.Bakery)
            {
                containsBakery = true;
                break;
            }
        }
        
        if (containsBakery && resourceDiff.ContainsKey(ResourceType.Bread) && resourceDiff.ContainsKey(ResourceType.Wheat))
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            if (r >= BAKERS_DOZEN_RELIC_PROBABILITY)
            {
                outResourceDiff[ResourceType.Wheat] += 1;
                return true;
            }
            else return false;
        }
        else return false;
    }

    public override void SerializeRelic()
    {
        
    }
}
