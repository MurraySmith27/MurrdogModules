using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Augment
{
    public class AugmentTriggeredData
    {
        public long NumGoldAdded = 0;
        public int NumAdditionalTileTriggers = 0;
    }
    
    public virtual bool OnTileTriggered(Vector2Int tilePosition, out AugmentTriggeredData triggeredData)
    {
        triggeredData = new();
        return false;
    }

    public virtual bool OnTileMaintenanceCostComputed(Vector2Int tilePosition, int maintenanceCostOfThisTile, out int newMaintenanceCostOfThisTile)
    {
        newMaintenanceCostOfThisTile = maintenanceCostOfThisTile;
        return false;
    }
}
