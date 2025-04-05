using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        
        BloomingHarvestController.Instance.OnTileProcessStart -= OnTileProcessStart;
        BloomingHarvestController.Instance.OnTileProcessStart += OnTileProcessStart;

        BloomingHarvestController.Instance.OnTileResourceChangeStart -= OnTileResourceChangeStart;
        BloomingHarvestController.Instance.OnTileResourceChangeStart += OnTileResourceChangeStart;
        
        BloomingHarvestController.Instance.OnTileResourceChangeEnd -= OnTileResourceChangeEnd;
        BloomingHarvestController.Instance.OnTileResourceChangeEnd += OnTileResourceChangeEnd;

        BloomingHarvestController.Instance.OnTileBonusTickStart -= OnTileBonusTickStart;
        BloomingHarvestController.Instance.OnTileBonusTickStart += OnTileBonusTickStart;
        
        BloomingHarvestController.Instance.OnTileBonusTickEnd -= OnTileBonusTickEnd;
        BloomingHarvestController.Instance.OnTileBonusTickEnd += OnTileBonusTickEnd;
    }

    private void OnDestroy()
    {
        if (BloomingHarvestController.IsAvailable)
        {
            BloomingHarvestController.Instance.OnHarvestStart -= OnHarvestStart;
            BloomingHarvestController.Instance.OnHarvestEnd -= OnHarvestEnd;
            BloomingHarvestController.Instance.OnTileHarvestStart -= OnTileHarvestStart;
            BloomingHarvestController.Instance.OnTileProcessStart -= OnTileProcessStart;
            BloomingHarvestController.Instance.OnTileResourceChangeStart -= OnTileResourceChangeStart;
            BloomingHarvestController.Instance.OnTileResourceChangeEnd -= OnTileResourceChangeEnd;
            BloomingHarvestController.Instance.OnTileBonusTickStart -= OnTileBonusTickStart;
            BloomingHarvestController.Instance.OnTileBonusTickEnd -= OnTileBonusTickEnd;
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

    private void OnTileResourceChangeStart(Vector2Int tilePosition)
    {
        TileVisuals tileInstanceAtPosition = MapVisualsController.Instance.GetTileInstanceAtPosition(tilePosition);

        if (tileInstanceAtPosition != null)
        {
            tileInstanceAtPosition.StartTileHarvestAnimation();
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
    
    private void OnTileHarvestStart(Vector2Int position, Dictionary<ResourceType, int> resourcesChange)
    {
        TryAnimateTile(position, resourcesChange);
    }

    private void OnTileProcessStart(Vector2Int position, Dictionary<ResourceType, int> resourcesChange)
    {
        if (resourcesChange.Values.ToList().FindIndex((int value) => { return value != 0;}) != -1)
        {
            TryAnimateTile(position, resourcesChange);
        }
    }

    private void TryAnimateTile(Vector2Int position, Dictionary<ResourceType, int> resourcesChange)
    {
        TileVisuals tileInstanceAtPosition = MapVisualsController.Instance.GetTileInstanceAtPosition(position);
        
        tileInstanceAtPosition.TriggerTileHarvestAnimation();
        foreach (ResourceType resourceType in resourcesChange.Keys)
        {
            if (resourcesChange[resourceType] != 0)
            {
                if (tileInstanceAtPosition != null)
                {
                    OnTileHarvestAnimationTriggered?.Invoke(position);
                }
                
                break;
            }
        }
    }

    private void OnTileBonusTickStart(Vector2Int position)
    {
        TileVisuals tileInstanceAtPosition = MapVisualsController.Instance.GetTileInstanceAtPosition(position);
        
        tileInstanceAtPosition.TriggerBonusTickAnimation();
    }

    private void OnTileBonusTickEnd(Vector2Int position)
    {
        
    }
    
}
