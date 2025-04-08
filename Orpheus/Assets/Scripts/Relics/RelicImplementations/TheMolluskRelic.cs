using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheMolluskRelic : Relic
{
    private const int THE_MOLLUSK_RELIC_STONE_PER_FISH_FARM = 1;
    
    public override bool OnBuildingConstructed(Vector2Int position, BuildingType buildingType, out AdditionalTriggeredArgs args)
    {
        args = new();

        if (buildingType == BuildingType.FishFarm)
        {
            MapSystem.Instance.AddResourcesToTile(position, ResourceType.Stone, THE_MOLLUSK_RELIC_STONE_PER_FISH_FARM);
            args.IntArg = 1;
            return true;
        }
        else return false;
        
    }

    public override void SerializeRelic()
    {
        
    }
}
