using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AssetManager
{
    public static List<TileDescriptor> GetTileData()
    {
        TileDataSO tileData = Resources.Load<TileDataSO>("TileData");

        if (tileData == null)
        {
            Debug.LogError("Error loading in tile data: TileData is null");
            return new();
        }
        else return tileData.tiles;
    }
}
