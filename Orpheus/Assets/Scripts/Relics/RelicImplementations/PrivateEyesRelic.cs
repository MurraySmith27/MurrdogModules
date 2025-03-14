using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivateEyesRelic : Relic
{
    private const int CORN_HARVESTED_PER_FREE_TILE = 10;
    
    private Dictionary<Guid, int> _totalCornHarvestedPerCity = new Dictionary<Guid, int>();
    
    public bool OnResourcesHarvested(Dictionary<ResourceType, int> resourcesToBeHarvested, Guid cityGuid, out  Dictionary<ResourceType, int> outResourcesToBeHarvested, out AdditionalRelicTriggeredArgs args)
    {
        outResourcesToBeHarvested = resourcesToBeHarvested;
        args = new();
        
        int cornCount = 0;
        if (resourcesToBeHarvested.ContainsKey(ResourceType.Corn))
        {
            cornCount = resourcesToBeHarvested[ResourceType.Corn];
        }

        if (cornCount > 0)
        {
            if (_totalCornHarvestedPerCity.ContainsKey(cityGuid))
            {
                _totalCornHarvestedPerCity.Add(cityGuid, 0);
            }
            
            _totalCornHarvestedPerCity[cityGuid] += cornCount;
            
            args.IntArg = cornCount;
            return true;
        }
        else return false;
    }
    
    public bool OnPhaseChanged(GamePhases phase, out AdditionalRelicTriggeredArgs args)
    {
        args = new();
        if (phase == GamePhases.BloomingEndStep)
        {
            List<Guid> cityGuids = MapSystem.Instance.GetAllCityGuids();

            bool builtTile = false;

            List<Vector2Int> newTileLocations = new List<Vector2Int>();
            
            foreach (Guid cityGuid in cityGuids)
            {
                if (_totalCornHarvestedPerCity.ContainsKey(cityGuid) && _totalCornHarvestedPerCity[cityGuid] >= CORN_HARVESTED_PER_FREE_TILE)
                {
                    _totalCornHarvestedPerCity[cityGuid] = 0;

                    Vector2Int newTileLocation = MapSystem.Instance.AddRandomTileToCity(cityGuid);

                    newTileLocations.Add(newTileLocation);
                    
                    builtTile = true;
                }
            }

            args.Vector2IntListArgs = newTileLocations;
            
            return builtTile;
        }

        return false;
    }

    public override void SerializeRelic()
    {
        
    }
}
