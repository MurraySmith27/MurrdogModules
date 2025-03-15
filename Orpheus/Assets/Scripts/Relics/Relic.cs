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
    public virtual bool OnResourcesHarvested(Dictionary<ResourceType, int> resourcesToBeHarvested, Vector2Int position, out  Dictionary<ResourceType, int> outResourcesToBeHarvested, out AdditionalRelicTriggeredArgs args)
    {
        outResourcesToBeHarvested = resourcesToBeHarvested;
        args = new();
        return false;
    }
    
    //called when a tile processes resources, passed in is the state of currently harvested resources, returned is the modified state
    public virtual bool OnResourcesProcessed(Dictionary<ResourceType, int> resourceDiff, Vector2Int position, out Dictionary<ResourceType, int> outResourceDiff, out AdditionalRelicTriggeredArgs args)
    {
        outResourceDiff = resourceDiff;
        args = new();
        return false;
    }

    public virtual bool OnPhaseChanged(GamePhases phase, out AdditionalRelicTriggeredArgs args)
    {
        args = new();
        return false;
    }

    public virtual bool OnGoldInterestAdded(long coinTotalBefore, long interest, out long newInterest, out AdditionalRelicTriggeredArgs args)
    {
        newInterest = interest;
        args = new();
        return false;
    }

    //called when resources are being converted to food score, the passed in baseFoodScore is the score so far,
    //  so any returned score should be additive to that
    public virtual bool OnFoodScoreConversion(long baseFoodScore, Dictionary<ResourceType, int> resourcesToConvert, out long convertedFoodScore, out AdditionalRelicTriggeredArgs args)
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
