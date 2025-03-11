using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestAnimationController : Singleton<HarvestAnimationController>
{
    private void Start()
    {
        BloomingHarvestController.Instance.OnCityHarvestStart -= OnCityHarvestStart;
        BloomingHarvestController.Instance.OnCityHarvestStart += OnCityHarvestStart;
        
        BloomingHarvestController.Instance.OnTileHarvestStart -= OnTileHarvestStart;
        BloomingHarvestController.Instance.OnTileHarvestStart += OnTileHarvestStart;
    }

    private void OnDestroy()
    {
        if (BloomingHarvestController.IsAvailable)
        {
            BloomingHarvestController.Instance.OnTileHarvestStart -= OnTileHarvestStart;
            BloomingHarvestController.Instance.OnTileHarvestStart -= OnTileHarvestStart;
        }
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
