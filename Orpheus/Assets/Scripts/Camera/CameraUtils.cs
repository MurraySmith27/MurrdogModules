using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtils
{
    public static bool GetTilePositionFromMousePosition(Vector2 mousePos, Camera camera, out Vector2Int tilePosition)
    {
        tilePosition = new Vector2Int();
        Vector3 pointOnGroundPlane;
        if (CameraUtils.GetPointOnPlaneFromMousePosition(mousePos, camera, out pointOnGroundPlane))
        {
            Vector3 tileSpacePos = HexUtils.WorldSpaceToTileSpace(pointOnGroundPlane);
            tilePosition = new Vector2Int(Mathf.RoundToInt(tileSpacePos.x / GameConstants.TILE_SIZE), Mathf.RoundToInt(tileSpacePos.z / GameConstants.TILE_SIZE));
            return true;
        }
        else return false;
    }

    public static bool GetPointOnPlaneFromMousePosition(Vector2 mousePos, Camera camera, out Vector3 pointOnGroundPlane)
    {
        Vector3 screenSpacePos = new Vector3(mousePos.x, mousePos.y, 10f);

        Vector3 directionFromEye =
            (camera.ScreenToWorldPoint(screenSpacePos) - camera.transform.position);
        
        return GetPointOnGroundPlaneAlongRay(camera.transform.position, directionFromEye, out pointOnGroundPlane);
    }

    public static bool GetPointOnGroundPlaneAlongRay(Vector3 origin, Vector3 direction, out Vector3 pointOnGroundPlane)
    {
        pointOnGroundPlane = new Vector3();
        if (direction.y == 0)
        {
            return false;
        }
        
        float t = -origin.y / direction.normalized.y;

        if (t > 0)
        {
            pointOnGroundPlane = direction.normalized * t + origin;
            return true;
        }
        else return false;
    }
    
    public static bool GetCameraPointOnPlane(Camera camera, Vector2 normalizedScreenSpaceCoords, out Vector3 centerPointOnPlane)
    {
        return GetPointOnPlaneFromMousePosition(normalizedScreenSpaceCoords * new Vector2(Screen.width, Screen.height), camera, out centerPointOnPlane);
    }

    public static bool GetCameraCenterPointOnPlane(Camera camera, out Vector3 centerPointOnPlane)
    {
        return GetPointOnPlaneFromMousePosition(new Vector2(Screen.width / 2f, Screen.height / 2f), camera, out centerPointOnPlane);
    }
}
