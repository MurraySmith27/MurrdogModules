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
        BloomingHarvestController.Instance.OnHarvestStart -= LockCamera;
        BloomingHarvestController.Instance.OnHarvestStart += LockCamera;
        
        BloomingHarvestController.Instance.OnCityHarvestStart -= OnCityHarvestStart;
        BloomingHarvestController.Instance.OnCityHarvestStart += OnCityHarvestStart;

        BloomingHarvestController.Instance.OnCityHarvestEnd -= OnCityHarvestEnd;
        BloomingHarvestController.Instance.OnCityHarvestEnd += OnCityHarvestEnd;
        
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

        BloomingResourceConversionController.Instance.OnResourceConversionStart -= LockCamera;
        BloomingResourceConversionController.Instance.OnResourceConversionStart += LockCamera;

        BloomingResourceConversionController.Instance.OnResourceConversionEnd -= UnlockCamera;
        BloomingResourceConversionController.Instance.OnResourceConversionEnd += UnlockCamera;
    }

    private void OnDestroy()
    {
        if (BloomingHarvestController.IsAvailable)
        {
            BloomingHarvestController.Instance.OnHarvestStart -= LockCamera;
            BloomingHarvestController.Instance.OnTileHarvestStart -= OnTileHarvestStart;
            BloomingHarvestController.Instance.OnTileProcessStart -= OnTileProcessStart;
            BloomingHarvestController.Instance.OnTileResourceChangeStart -= OnTileResourceChangeStart;
            BloomingHarvestController.Instance.OnTileResourceChangeEnd -= OnTileResourceChangeEnd;
            BloomingHarvestController.Instance.OnTileBonusTickStart -= OnTileBonusTickStart;
            BloomingHarvestController.Instance.OnTileBonusTickEnd -= OnTileBonusTickEnd;
            BloomingResourceConversionController.Instance.OnResourceConversionStart -= LockCamera;
            BloomingResourceConversionController.Instance.OnResourceConversionEnd -= UnlockCamera;
        }
    }

    private void LockCamera()
    {
        CameraController.Instance.SetCameraLock(true);
    }

    private void UnlockCamera()
    {
        CameraController.Instance.SetCameraLock(true);
    }
    
    private void OnCityHarvestStart(Guid cityGuid)
    {
        Vector2Int cityPosition = MapSystem.Instance.GetCityCenterPosition(cityGuid);
        
        CameraController.Instance.FocusPosition(MapUtils.GetTileWorldPositionFromGridPosition(cityPosition));
    }

    private void OnCityHarvestEnd(Guid cityGuid)
    {
        CameraController.Instance.FocusPosition(MapUtils.GetTileWorldPositionFromGridPosition(MapSystem.Instance.GetCityCenterPosition(cityGuid)));
    }

    private void OnTileResourceChangeStart(Vector2Int tilePosition)
    {
        CameraController.Instance.FocusPosition(MapUtils.GetTileWorldPositionFromGridPosition(tilePosition));
        
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
