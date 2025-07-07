using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Desert terrain provides + 1 yield to adjacent industrial tiles.
public class DesertTerrain : TerrainBase
{
    public override bool GetTerrainBonusTileYields(Vector2Int tilePosition, out List<(Vector2Int, int)> tileYieldBonuses)
    {
        List<Vector2Int> adjacentFoodTiles =
            TerrainBonusUtils.GetAdjacentTilesWithOuputResourceTag(tilePosition, ResourceTags.INDUSTRIAL);

        tileYieldBonuses = new();

        foreach (Vector2Int position in adjacentFoodTiles)
        {
            tileYieldBonuses.Add((position, 1));
        }
        
        return true;
    }

    public override void SerializeTerrain()
    {
        //TODO   
    }
}
