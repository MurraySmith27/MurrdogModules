using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Relic
{
    public virtual RelicTypes GetType()
    {
        return RelicTypes.NONE;
    }
    
    //ALL OF THESE METHODS RETURN TRUE IF THEY'RE OVERRIDDEN IN THE SUB CLASS, FALSE OTHERWISE
    
    //called when a tile is harvesting NEW resources. passed in are the resources that are harvested, returned are the modified resources
    public virtual bool OnResourcesHarvested(Dictionary<ResourceType, int> resourcesToBeHarvested, out  Dictionary<ResourceType, int> outResourcesToBeHarvested, out AdditionalRelicTriggeredArgs args)
    {
        outResourcesToBeHarvested = resourcesToBeHarvested;
        args = new();
        return false;
    }
    
    //called when a tile processes resources, passed in is the state of currently harvested resources, returned is the modified state
    public virtual bool OnResourcesProcessed(Dictionary<ResourceType, int> resourceState, out Dictionary<ResourceType, int> outResourceState, out AdditionalRelicTriggeredArgs args)
    {
        outResourceState = resourceState;
        args = new();
        return false;
    }

    public virtual bool OnPhaseChanged(GamePhases phase, out AdditionalRelicTriggeredArgs args)
    {
        args = new();
        return false;
    }

    public virtual bool OnGoldInterestAdded(int coinTotalBefore, int interest, out int newInterest, out AdditionalRelicTriggeredArgs args)
    {
        newInterest = interest;
        args = new();
        return false;
    }

    //called when resources are being converted to food score, the passed in baseFoodScore is the score so far,
    //  so any returned score should be additive to that
    public virtual bool OnFoodScoreConversion(int baseFoodScore, Dictionary<ResourceType, int> resourcesToConvert, out int convertedFoodScore, out AdditionalRelicTriggeredArgs args)
    {
        convertedFoodScore = baseFoodScore;
        args = new();
        return false;
    }

    public virtual bool OnBuildingConstructed(Vector2Int position, BuildingType buildingType, out AdditionalRelicTriggeredArgs args)
    {
        args = new();
        return false;
    }

    public abstract void SerializeRelic();
}
