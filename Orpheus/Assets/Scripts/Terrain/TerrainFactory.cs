using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFactory
{
    public TerrainBase CreateTerrain(TileType type)
    {
        switch (type)
        {
            case TileType.None:
                return null;
            case TileType.Grass:
                return new GrassTerrain();
                break;
            case TileType.Desert:
                return new DesertTerrain();
                break;
            case TileType.Water:
                return new WaterTerrain();
                break;
        }

        return null;
    }
}