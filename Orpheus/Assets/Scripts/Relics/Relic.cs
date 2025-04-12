using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Relic
{
    //ALL OF THESE METHODS RETURN TRUE IF THEY'RE OVERRIDDEN IN THE SUB CLASS, FALSE OTHERWISE
    
    //called when a tile is harvesting NEW resources. passed in are the resources that are harvested, returned are the modified resources
    public virtual bool OnResourcesHarvested(Dictionary<ResourceType, int> resourcesToBeHarvested, Vector2Int position, out  Dictionary<ResourceType, int> outResourcesToBeHarvested, out AdditionalTriggeredArgs args)
    {
        outResourcesToBeHarvested = resourcesToBeHarvested;
        args = new();
        return false;
    }
    
    //called when a tile processes resources, passed in is the state of currently harvested resources, returned is the modified state
    public virtual bool OnResourcesProcessed(Dictionary<ResourceType, int> resourceDiff, Vector2Int position, out Dictionary<ResourceType, int> outResourceDiff, out AdditionalTriggeredArgs args)
    {
        outResourceDiff = resourceDiff;
        args = new();
        return false;
    }

    public virtual bool OnPhaseChanged(GamePhases phase, out AdditionalTriggeredArgs args)
    {
        args = new();
        return false;
    }

    public virtual bool OnGoldInterestAdded(long coinTotalBefore, long interest, out long newInterest, out AdditionalTriggeredArgs args)
    {
        newInterest = interest;
        args = new();
        return false;
    }

    public virtual bool OnConvertResourceToFoodScore(ResourceType resourceType, long quantitySoFar, double multSoFar, out long quantityChange, out double multChange, out AdditionalTriggeredArgs args)
    {
        quantityChange = 0;
        multChange = 0d;
        args = new();
        return false;
    }

    public virtual bool OnFoodHarvestedQuantityCalculated(long quantitySoFar, ResourceType resourceType,
        out long foodQuantityDifference, out AdditionalTriggeredArgs args)
    {
        foodQuantityDifference = 0;
        args = new();
        return false;
    }
    
    public virtual bool OnFoodHarvestedMultCalculated(double multSoFar, ResourceType resourceType,
        out double multDifference, out AdditionalTriggeredArgs args)
    {
        multDifference = 0;
        args = new();
        return false;
    }

    //called when resources are being converted to food score, the passed in baseFoodScore is the score so far,
    //  so any returned score should be additive to that
    public virtual bool OnFoodScoreConversionComplete(long baseFoodScore, Dictionary<ResourceType, int> resourcesToConvert, out long convertedFoodScore, out AdditionalTriggeredArgs args)
    {
        convertedFoodScore = baseFoodScore;
        args = new();
        return false;
    }

    public virtual bool OnBuildingConstructed(Vector2Int position, BuildingType buildingType, out AdditionalTriggeredArgs args)
    {
        args = new();
        return false;
    }
    
    public virtual bool OnHarvestStart(out AdditionalTriggeredArgs args)
    {
        args = new();
        
        return false;
    }

    public abstract void SerializeRelic();
}
