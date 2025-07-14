using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloAugment : Augment
{
    public override bool OnTileTriggered(Vector2Int tilePosition, out AugmentTriggeredData triggeredData)
    {
        triggeredData = new();
        triggeredData.NumAdditionalTileTriggers = 1;
        return true;
    }

}
