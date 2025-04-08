using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivateEyesRelic : Relic
{
    private const int CORN_HARVESTED_PER_FREE_TILE = 10;
    
    private Dictionary<Guid, int> _totalCornHarvestedPerCity = new Dictionary<Guid, int>();
    
    public override bool OnResourcesHarvested(Dictionary<ResourceType, int> resourcesToBeHarvested, Vector2Int position, out  Dictionary<ResourceType, int> outResourcesToBeHarvested, out AdditionalTriggeredArgs args)
    { 
        outResourcesToBeHarvested = resourcesToBeHarvested;
        args = new();

        Guid cityGuid;
        
        if (!MapSystem.Instance.GetCityGuidFromTile(position, out cityGuid))
        {
            Debug.LogError($"City guid is null! tile at {position} is not in a city!");
            return false;
        }
        
        int cornCount = 0;
        if (resourcesToBeHarvested.ContainsKey(ResourceType.Corn))
        {
            cornCount = resourcesToBeHarvested[ResourceType.Corn];
        }

        if (cornCount > 0)
        {
            if (!_totalCornHarvestedPerCity.ContainsKey(cityGuid))
            {
                _totalCornHarvestedPerCity.Add(cityGuid, 0);
            }
            
            _totalCornHarvestedPerCity[cityGuid] += cornCount;
            
            args.IntArg = cornCount;
            return true;
        }
        else return false;
    }
    
    public override bool OnPhaseChanged(GamePhases phase, out AdditionalTriggeredArgs args)
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

            args.GuidListArgs = cityGuids;
            args.Vector2IntListArgs = newTileLocations;
            
            return builtTile;
        }

        return false;
    }

    public override void SerializeRelic()
    {
        
    }
}
