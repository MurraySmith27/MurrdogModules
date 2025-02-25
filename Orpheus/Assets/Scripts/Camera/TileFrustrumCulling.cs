using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFrustrumCulling : Singleton<TileFrustrumCulling>
{
    public delegate void TileCullingUpdatedDelegate(int x, int y, int width, int height);
    public event TileCullingUpdatedDelegate OnTileCullingUpdated;
    
    [SerializeField] private Camera cullingCamera;

    [SerializeField] private int cullingTilePadding = 1;
    
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    [SerializeField] private bool enableFrustrumCulling = true;
#endif 
    
    public void UpdateTileCulling()
    {
        
        Vector2Int mapDimensions = MapSystem.Instance.GetMapDimensions();
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (!enableFrustrumCulling)
        {
            OnTileCullingUpdated?.Invoke(0, 0, mapDimensions.x, mapDimensions.y);
            return;
        }
#endif
        
        //cast rays from the four corners of the camera frustrum to the far render plane,
        //the tiles hit are the ones that don't get culled, and anything interior.
        
        Vector3 cameraPos = cullingCamera.transform.position;
        //order is bottom left, top left, top right, bottom right
        Vector3[] frustrumCorners = GetCameraFrustrumCorners();
        
        RectInt mapBounds = new RectInt(0, 0, 0, 0);

        for (int i = 0; i < 4; i++)
        {
            Vector3 hit;
            if (CameraUtils.GetPointOnGroundPlaneAlongRay(cameraPos, frustrumCorners[i] - cameraPos, out hit))
            {
                Vector2Int gridPos = MapUtils.GetGridPositionFromWorldPosition(hit);

                if (i == 0)
                {
                    mapBounds = new RectInt(gridPos, new Vector2Int(0, 0));
                }
                else
                {
                    if (gridPos.x < mapBounds.xMin)
                    {
                        mapBounds.xMin = gridPos.x;
                    }
                    else if (gridPos.x > mapBounds.xMax)
                    {
                        mapBounds.xMax = gridPos.x;
                    }

                    if (gridPos.y < mapBounds.yMin)
                    {
                        mapBounds.yMin = gridPos.y;
                    }
                    else if (gridPos.y > mapBounds.yMax)
                    {
                        mapBounds.yMax = gridPos.y;
                    }
                }
            }
        }

        if (mapBounds.width == 0)
        {
            mapBounds.xMax = mapDimensions.x;
        }
        
        if (mapBounds.height == 0)
        {
            mapBounds.yMax = mapDimensions.y;
        }
        
        OnTileCullingUpdated?.Invoke(Mathf.Max(mapBounds.x - cullingTilePadding, 0), Mathf.Max(mapBounds.y - cullingTilePadding, 0), mapBounds.width + 1 + cullingTilePadding, mapBounds.height + 1 + cullingTilePadding);
    }

    private Vector3[] GetCameraFrustrumCorners()
    {
        Vector3[] frustrumCorners = new Vector3[4];
        
        frustrumCorners[0] = cullingCamera.ViewportToWorldPoint(new Vector3(0, 0, cullingCamera.farClipPlane));
        frustrumCorners[1] = cullingCamera.ViewportToWorldPoint(new Vector3(0, 1, cullingCamera.farClipPlane));
        frustrumCorners[2] = cullingCamera.ViewportToWorldPoint(new Vector3(1, 1, cullingCamera.farClipPlane));
        frustrumCorners[3] = cullingCamera.ViewportToWorldPoint(new Vector3(1, 0, cullingCamera.farClipPlane));

        return frustrumCorners;
    }
}
