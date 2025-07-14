using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenAugment : Augment
{
    public override bool OnTileMaintenanceCostComputed(Vector2Int tilePosition, int maintenanceCostOfThisTile, out int newMaintenanceCostOfThisTile)
    {
        newMaintenanceCostOfThisTile = 0;
        return true;
    }
}
