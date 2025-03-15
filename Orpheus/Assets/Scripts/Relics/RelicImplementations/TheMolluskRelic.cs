using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheMolluskRelic : Relic
{
    private const int THE_MOLLUSK_RELIC_STONE_PER_FISH_FARM = 1;
    
    public virtual bool OnBuildingConstructed(Vector2Int position, BuildingType buildingType, out AdditionalRelicTriggeredArgs args)
    {
        args = new();

        if (buildingType == BuildingType.FishFarm)
        {
            MapSystem.Instance.AddResourcesToTile(position, ResourceType.Stone, 1);
            args.IntArg = 1;
            return true;
        }
        else return false;
        
    }

    public override void SerializeRelic()
    {
        
    }
}
