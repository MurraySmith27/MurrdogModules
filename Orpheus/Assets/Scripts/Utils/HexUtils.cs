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
                c_cached =  (Mathf.Sqrt(Mathf.Pow(GameConstants.HEX_SIDE_LENGTH, 2f) + Mathf.Pow(w_cached, 2f)) - GameConstants.HEX_SIDE_LENGTH) * 0.5f;
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
                gridOffset_cached = new Vector2(w / 2f, GameConstants.HEX_SIDE_LENGTH + c);
            }
            
            return gridOffset_cached;
        }
    }

    public static Vector3 TileSpaceToWorldSpace(Vector3 tileSpacePos)
    {
          return new Vector3(tileSpacePos.x * gridOffset.x, 0, tileSpacePos.z * gridOffset.y);
    }
    
    public static Vector3 WorldSpaceToTileSpace(Vector3 worldSpacePos)
    {
        return new Vector3(worldSpacePos.x / gridOffset.x, 0, worldSpacePos.z / gridOffset.y);
    }
}
