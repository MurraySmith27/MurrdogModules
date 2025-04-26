using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexUtils
{
    private static float c_cached = 0f;
    private static float c
    {
        get
        {
            if (c_cached == 0f)
            { 
                c_cached =  (Mathf.Sqrt(Mathf.Pow(GameConstants.HEX_SIDE_LENGTH, 2f) + Mathf.Pow(w, 2f)) - GameConstants.HEX_SIDE_LENGTH) * 0.5f;
            }
            
            return c_cached;
        }
    }

    private static float w_cached = 0f;
    private static float w
    {
        get
        {
            if (w_cached == 0f)
            {
                w_cached = Mathf.Sqrt(3f) * GameConstants.HEX_SIDE_LENGTH;
            }
            
            return w_cached;
        }
    }

    private static Vector2 gridOffset_cached = Vector2.zero;

    public static Vector2 gridOffset
    {
        get
        {
            if (gridOffset_cached == Vector2.zero)
            {
                gridOffset_cached = new Vector2((GameConstants.HEX_SIDE_LENGTH + c), w / 2f);
            }
            
            return gridOffset_cached;
        }
    }

    public static Vector3 TileSpaceToWorldSpace(Vector3 tileSpacePos)
    {
          return new Vector3((gridOffset.x) * tileSpacePos.x, 0, w * tileSpacePos.z + tileSpacePos.x * gridOffset.y);
    }
    
    public static Vector3 WorldSpaceToTileSpace(Vector3 worldSpacePos)
    {
        float xPos = worldSpacePos.x / (gridOffset.x);
        return new Vector3(xPos, 0, (worldSpacePos.z - xPos * gridOffset.y) / w);
    }
}
