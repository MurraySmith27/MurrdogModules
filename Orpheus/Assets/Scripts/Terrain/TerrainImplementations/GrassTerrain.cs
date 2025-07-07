using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Grass terrain provides + 1 yield to adjacent food tiles.
public class GrassTerrain : TerrainBase
{
    public override bool GetTerrainBonusTileYields(Vector2Int tilePosition, out List<(Vector2Int, int)> tileYieldBonuses)
    {
        List<Vector2Int> adjacentFoodTiles =
            TerrainBonusUtils.GetAdjacentTilesWithOuputResourceTag(tilePosition, ResourceTags.FOOD);

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
