using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFrustrumCulling : Singleton<TileFrustrumCulling>
{
    public delegate void TileCullingUpdatedDelegate(int x, int y, int width, int height);
    public event TileCullingUpdatedDelegate OnTileCullingUpdated;
    
    [SerializeField] private Camera cullingCamera;

    private void Start()
    {
        MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
        MapSystem.Instance.OnMapChunkGenerated += OnMapChunkGenerated;
    }

    private void OnDestroy()
    {
        if (MapSystem.IsAvailable)
        {
            MapSystem.Instance.OnMapChunkGenerated -= OnMapChunkGenerated;
        }
    }

    private void OnMapChunkGenerated(int x, int y, int width, int height)
    {
        UpdateTileCulling();   
    }

    public void UpdateTileCulling()
    {
        //cast rays from the four corners of the camera frustrum to the far render plane,
        //the tiles hit are the ones that don't get culled, and anything interior.
        
        Vector3 cameraPos = cullingCamera.transform.position;
        //order is bottom left, top left, top right, bottom right
        Vector3[] frustrumCorners = GetCameraFrustrumCorners();

        Vector2Int mapDimensions = MapSystem.Instance.GetMapDimensions();

        Vector2Int bottomLeft = new Vector2Int(mapDimensions.y, 0);
        Vector3 bottomLeftHit;
        if (CameraUtils.GetPointOnGroundPlaneAlongRay(cameraPos, frustrumCorners[0] - cameraPos, out bottomLeftHit))
        {
            bottomLeft = MapUtils.GetGridPositionFromWorldPosition(bottomLeftHit);
        }
        
        Debug.DrawRay(cameraPos, frustrumCorners[0] - cameraPos, Color.red, 20f);
        
        Vector2Int topRight = new Vector2Int(0, mapDimensions.x);
        Vector3 topRightHit;
        if (CameraUtils.GetPointOnGroundPlaneAlongRay(cameraPos, frustrumCorners[2] - cameraPos, out topRightHit))
        {
            topRight = MapUtils.GetGridPositionFromWorldPosition(topRightHit);
        }
        
        Debug.DrawRay(cameraPos, frustrumCorners[2] - cameraPos, Color.blue, 20f);
        
        RectInt cullingBounds = new RectInt(bottomLeft, topRight - bottomLeft);
        
        OnTileCullingUpdated?.Invoke(cullingBounds.x, cullingBounds.y, cullingBounds.width, cullingBounds.height);
    }

    private Vector3[] GetCameraFrustrumCorners()
    {
        Vector3[] frustrumCorners = new Vector3[4];
        cullingCamera.CalculateFrustumCorners(new Rect(0,0,1,1), cullingCamera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustrumCorners);

        // for (int i = 0; i < 4; i++)
        // {
        //     frustrumCorners[i] = cullingCamera.ViewportToWorldPoint(frustrumCorners[i]);
        // }

        return frustrumCorners;
    }
}
