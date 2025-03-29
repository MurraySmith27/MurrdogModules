using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestAnimationController : Singleton<HarvestAnimationController>
{
    public event Action<Vector2Int> OnTileHarvestAnimationTriggered;
    
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
        
        BloomingHarvestController.Instance.OnTileProcessStart -= TryAnimateTile;
        BloomingHarvestController.Instance.OnTileProcessStart += TryAnimateTile;

        BloomingHarvestController.Instance.OnTileResourceChangeEnd -= OnTileResourceChangeEnd;
        BloomingHarvestController.Instance.OnTileResourceChangeEnd += OnTileResourceChangeEnd;
    }

    private void OnDestroy()
    {
        if (BloomingHarvestController.IsAvailable)
        {
            BloomingHarvestController.Instance.OnHarvestStart -= OnHarvestStart;
            BloomingHarvestController.Instance.OnHarvestEnd -= OnHarvestEnd;
            BloomingHarvestController.Instance.OnTileHarvestStart -= OnTileHarvestStart;
            BloomingHarvestController.Instance.OnTileProcessStart -= TryAnimateTile;
            BloomingHarvestController.Instance.OnTileResourceChangeEnd -= OnTileResourceChangeEnd;
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
            tileInstanceAtPosition.StartTileHarvestAnimation(resourcesChange);
            
            TryAnimateTile(position, resourcesChange);
        }
    }

    private void TryAnimateTile(Vector2Int position, Dictionary<ResourceType, int> resourcesChange)
    {
        foreach (ResourceType resourceType in resourcesChange.Keys)
        {
            if (resourcesChange[resourceType] != 0)
            {
                TileVisuals tileInstanceAtPosition = MapVisualsController.Instance.GetTileInstanceAtPosition(position);

                if (tileInstanceAtPosition != null)
                {
                    tileInstanceAtPosition.TriggerTileHarvestAnimation(resourcesChange);
                    OnTileHarvestAnimationTriggered?.Invoke(position);
                }
                
                break;
            }
        }
    }
    
    private void OnTileResourceChangeEnd(Vector2Int position)
    {
        TileVisuals tileInstanceAtPosition = MapVisualsController.Instance.GetTileInstanceAtPosition(position);

        if (tileInstanceAtPosition != null)
        {
            tileInstanceAtPosition.EndTileHarvestAnimation();
        }
    }
}
