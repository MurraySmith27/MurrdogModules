using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class BakersDozenRelic : Relic
{
    private const float BAKERS_DOZEN_RELIC_PROBABILITY = 0.5f;
    
    public override bool OnResourcesProcessed(Dictionary<ResourceType, int> totalResourceDiff, 
        Vector2Int position, out Dictionary<ResourceType, int> outResourceDiff, 
        out Dictionary<PersistentResourceType, int> outPersistentResourcesDiff, out AdditionalTriggeredArgs args)
    {
        args = new();

        outResourceDiff = new();
        outPersistentResourcesDiff = new();

        List<TileBuilding> buildingTiles = MapSystem.Instance.GetBuildingsOnTile(position);

        bool containsBakery = false;
        
        foreach (TileBuilding building in buildingTiles) {

            if (building.Type == BuildingType.Bakery)
            {
                containsBakery = true;
                break;
            }
        }
        
        if (containsBakery && totalResourceDiff.ContainsKey(ResourceType.Bread) && totalResourceDiff.ContainsKey(ResourceType.Wheat))
        {
            float r = UnityEngine.Random.Range(0f, 1f);
            if (r >= BAKERS_DOZEN_RELIC_PROBABILITY)
            {
                if (!outResourceDiff.ContainsKey(ResourceType.Wheat))
                {
                    outResourceDiff.Add(ResourceType.Wheat, 0);
                }
                
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
