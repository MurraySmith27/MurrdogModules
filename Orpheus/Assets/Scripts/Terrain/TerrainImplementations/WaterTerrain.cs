using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//water terrain provides no adjacency bonuses, but +3 gold each round.
public class WaterTerrain : TerrainBase
{
    public override bool GetTerrainBonusTileYields(Vector2Int tilePosition, out List<(Vector2Int, int)> tileYieldBonuses)
    {
        tileYieldBonuses = new();
        return false;
    }

    public override void SerializeTerrain()
    {
        //TODO   
    }
}
