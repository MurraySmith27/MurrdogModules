using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapUtils
{

    public static Vector3 GetTileWorldPositionFromGridPosition(Vector2Int gridPosition)
    {
        return new Vector3((gridPosition.x) * GameConstants.TILE_SIZE, 0, (gridPosition.y) * GameConstants.TILE_SIZE);
    }
    
    public static Vector3 GetTileCenterFromPlanePosition(Vector3 worldPosition)
    {
        return new Vector3(
                   Mathf.Round(worldPosition.x / GameConstants.TILE_SIZE),
                   0,
                   Mathf.Round(worldPosition.z / GameConstants.TILE_SIZE)) * GameConstants.TILE_SIZE;
    }

    public static Vector2Int GetGridPositionFromWorldPosition(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x / GameConstants.TILE_SIZE),
            Mathf.RoundToInt(worldPosition.z / GameConstants.TILE_SIZE));
    }

    public static Vector3 GetWorldPostiionFromPlanePosition(Vector3 planePosition)
    {
        return new Vector3(
            (planePosition.x) * GameConstants.TILE_SIZE, 
            0, 
            (planePosition.z) * GameConstants.TILE_SIZE
        );
    }
}
