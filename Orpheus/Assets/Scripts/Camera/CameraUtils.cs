using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtils
{
    public static Vector2Int GetTilePositionFromMousePosition(Vector2 mousePos, Camera camera)
    {
        Vector3 pointOnGroundPlane = GetPointOnPlaneFromMousePosition(mousePos, camera);
        
        return new Vector2Int(
            Mathf.FloorToInt(pointOnGroundPlane.x / GameConstants.TILE_SIZE),
            Mathf.FloorToInt(pointOnGroundPlane.z / GameConstants.TILE_SIZE));
    }

    public static Vector3 GetPointOnPlaneFromMousePosition(Vector2 mousePos, Camera camera)
    {
        Vector3 screenSpacePos = new Vector3(mousePos.x, mousePos.y, 10f);

        Vector3 directionFromEye =
            (camera.ScreenToWorldPoint(screenSpacePos) - camera.transform.position).normalized;

        float t = -camera.transform.position.y / directionFromEye.y;

        return directionFromEye * t + camera.transform.position;        
    }

    public static Vector3 GetCameraCenterPointOnPlane(Camera camera)
    {
        return GetPointOnPlaneFromMousePosition(new Vector2(Screen.width / 2f, Screen.height / 2f), camera);
    }
}
