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
        PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        PhaseStateMachine.Instance.OnPhaseChanged += OnPhaseChanged;
        
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

        if (PhaseStateMachine.IsAvailable)
        {
            PhaseStateMachine.Instance.OnPhaseChanged -= OnPhaseChanged;
        }
    }

    private void OnPhaseChanged(GamePhases gamePhases)
    {
        if (gamePhases == GamePhases.BloomingUpkeep)
        {
            LockCamera();
        }
        else if (gamePhases == GamePhases.BloomingEndStep)
        {
            UnlockCamera();
        }
    }

    private void LockCamera()
    {
        Guid cityGuid = MapSystem.Instance.GetAllCityGuids()[0];
        
        Vector2Int cityPosition = MapSystem.Instance.GetCityCenterPosition(cityGuid);
        
        CameraController.Instance.FocusPosition(MapUtils.GetTileWorldPositionFromGridPosition(cityPosition));
        
        CameraController.Instance.SetCameraLock(true);
    }

    private void UnlockCamera()
    {
        CameraController.Instance.SetCameraLock(false);
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
        TryAnimateTile(position, (resourcesChange, new()));
    }

    private void OnTileProcessStart(Vector2Int position, (Dictionary<ResourceType, int>, Dictionary<PersistentResourceType, int>) resourcesChange)
    {
        if (resourcesChange.Item1.Values.ToList().FindIndex((int value) => { return value != 0;}) != -1 || resourcesChange.Item2.Values.ToList().FindIndex((int value) => { return value != 0;}) != -1)
        {
            TryAnimateTile(position, resourcesChange);
        }
    }

    private void TryAnimateTile(Vector2Int position, (Dictionary<ResourceType, int>, Dictionary<PersistentResourceType, int>) resourcesChange)
    {
        TileVisuals tileInstanceAtPosition = MapVisualsController.Instance.GetTileInstanceAtPosition(position);
        
        tileInstanceAtPosition.TriggerTileHarvestAnimation();
        foreach (ResourceType resourceType in resourcesChange.Item1.Keys)
        {
            if (resourcesChange.Item1[resourceType] != 0)
            {
                if (tileInstanceAtPosition != null)
                {
                    OnTileHarvestAnimationTriggered?.Invoke(position);
                }

                return;
            }
        }
        
        foreach (PersistentResourceType resourceType in resourcesChange.Item2.Keys)
        {
            if (resourcesChange.Item2[resourceType] != 0)
            {
                if (tileInstanceAtPosition != null)
                {
                    OnTileHarvestAnimationTriggered?.Invoke(position);
                }
                
                return;
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
