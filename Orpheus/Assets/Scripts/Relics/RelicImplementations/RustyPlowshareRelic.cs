using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RustyPlowshareRelic : Relic
{
    public override bool OnPhaseChanged(GamePhases phase, out AdditionalRelicTriggeredArgs args)
    {
        args = new();
        
        if (phase == GamePhases.BuddingUpkeep)
        {
            List<Guid> cityGuids = MapSystem.Instance.GetAllCityGuids();
            
            Guid cityGuid = cityGuids[Random.Range(0, cityGuids.Count)];

            Vector2Int cityTile;
            if (MapSystem.Instance.GetUnoccupiedTileInCity(cityGuid, out cityTile))
            {
                return BuildingsController.Instance.TryPlaceBuilding(cityTile, BuildingType.WheatFarm);
            }
        }

        return false;
    }

    public override void SerializeRelic()
    {
        
    }
}
