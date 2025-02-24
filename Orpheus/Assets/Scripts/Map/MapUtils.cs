using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapUtils
{
    public static Vector3 GetTileCenterFromPlanePosition(Vector3 worldPosition)
    {
        float halfTileSize = GameConstants.TILE_SIZE / 2f;
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
}
