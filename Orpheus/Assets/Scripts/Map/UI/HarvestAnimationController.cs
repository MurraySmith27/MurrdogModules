using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestAnimationController : Singleton<HarvestAnimationController>
{
    private void Start()
    {
        BloomingHarvestController.Instance.OnHarvestStart -= OnHarvestStart;
        BloomingHarvestController.Instance.OnHarvestStart += OnHarvestStart;
        
        BloomingHarvestController.Instance.OnHarvestEnd -= OnHarvestEnd;
        BloomingHarvestController.Instance.OnHarvestEnd += OnHarvestEnd;
        
        BloomingHarvestController.Instance.OnCityHarvestStart -= OnCityHarvestStart;
        BloomingHarvestController.Instance.OnCityHarvestStart += OnCityHarvestStart;
        
        BloomingHarvestController.Instance.OnTileHarvestStart -= OnTileHarvestStart;
        BloomingHarvestController.Instance.OnTileHarvestStart += OnTileHarvestStart;
    }

    private void OnDestroy()
    {
        if (BloomingHarvestController.IsAvailable)
        {
            BloomingHarvestController.Instance.OnHarvestStart -= OnHarvestStart;
            BloomingHarvestController.Instance.OnHarvestEnd -= OnHarvestEnd;
            BloomingHarvestController.Instance.OnTileHarvestStart -= OnTileHarvestStart;
            BloomingHarvestController.Instance.OnTileHarvestStart -= OnTileHarvestStart;
        }
    }

    private void OnHarvestStart()
    {
        CameraController.Instance.SetCameraLock(true);
    }

    private void OnHarvestEnd()
    {
        CameraController.Instance.SetCameraLock(false);
    }
    
    private void OnCityHarvestStart(Guid cityGuid)
    {
        Vector2Int cityPostiion = MapSystem.Instance.GetCityCenterPosition(cityGuid);
        
        CameraController.Instance.FocusPosition(MapUtils.GetTileWorldPositionFromGridPosition(cityPostiion));
    }

    private void OnTileHarvestStart(Vector2Int position, Dictionary<ResourceType, int> resourcesChange)
    {
        TileVisuals tileInstanceAtPosition = MapVisualsController.Instance.GetTileInstanceAtPosition(position);

        if (tileInstanceAtPosition != null)
        {
            tileInstanceAtPosition.TriggerTileHarvestAnimation(resourcesChange);
        }
    }
}
