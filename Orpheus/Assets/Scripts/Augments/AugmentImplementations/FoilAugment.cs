using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoilAugment : Augment
{
    public override bool OnTileTriggered(Vector2Int tilePosition, out AugmentTriggeredData triggeredData)
    {
        triggeredData = new();
        triggeredData.NumGoldAdded = 1;
        return true;
    }
}
