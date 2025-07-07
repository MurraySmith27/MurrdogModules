using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainBase
{
    public virtual bool GetTerrainBonusTileYields(Vector2Int tilePosition, out List<(Vector2Int, int)> tileYieldBonuses)
    {
        tileYieldBonuses = new();
        return false;
    }

    public abstract void SerializeTerrain();
    
}
