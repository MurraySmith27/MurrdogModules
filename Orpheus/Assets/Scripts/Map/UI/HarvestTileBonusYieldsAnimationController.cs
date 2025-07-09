using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HarvestTileBonusYieldsAnimationController : Singleton<HarvestTileBonusYieldsAnimationController>
{
    public event Action<Vector2Int, Vector2Int, int> OnTileBonusYieldApplied;
    public event Action<Vector2Int> OnTileBonusYieldSourceStart;
    public event Action<Vector2Int> OnTileBonusYieldSourceEnd;
    
    private void Start()
    {
        BloomingTileBonusYieldsController.Instance.OnCityTileBonusYieldsStart -= OnCityBonusYieldsStart;
        BloomingTileBonusYieldsController.Instance.OnCityTileBonusYieldsStart += OnCityBonusYieldsStart;

        BloomingTileBonusYieldsController.Instance.OnCityTileBonusYieldsEnd -= OnCityHarvestEnd;
        BloomingTileBonusYieldsController.Instance.OnCityTileBonusYieldsEnd += OnCityHarvestEnd;
        
        BloomingTileBonusYieldsController.Instance.OnTileYieldBonusSourceStart -= OnTileYieldBonusSourceStart;
        BloomingTileBonusYieldsController.Instance.OnTileYieldBonusSourceStart += OnTileYieldBonusSourceStart;
        
        BloomingTileBonusYieldsController.Instance.OnTileYieldBonusSourceEnd -= OnTileYieldBonusSourceEnd;
        BloomingTileBonusYieldsController.Instance.OnTileYieldBonusSourceEnd += OnTileYieldBonusSourceEnd;

        BloomingTileBonusYieldsController.Instance.OnRelicTriggered -= OnRelicTriggered;
        BloomingTileBonusYieldsController.Instance.OnRelicTriggered += OnRelicTriggered;

        BloomingTileBonusYieldsController.Instance.OnTileYieldBonusGranted -= OnTileYieldBonusGranted;
        BloomingTileBonusYieldsController.Instance.OnTileYieldBonusGranted += OnTileYieldBonusGranted;
        
    }

    private void OnDestroy()
    {
        if (BloomingTileBonusYieldsController.IsAvailable)
        {
            BloomingTileBonusYieldsController.Instance.OnCityTileBonusYieldsStart -= OnCityBonusYieldsStart;
            BloomingTileBonusYieldsController.Instance.OnCityTileBonusYieldsEnd -= OnCityHarvestEnd;
        }
    }
    
    private void OnCityBonusYieldsStart(Guid cityGuid)
    {
        Vector2Int cityPosition = MapSystem.Instance.GetCityCenterPosition(cityGuid);
        
        CameraController.Instance.FocusPosition(MapUtils.GetTileWorldPositionFromGridPosition(cityPosition));
    }

    private void OnCityHarvestEnd(Guid cityGuid)
    {
        CameraController.Instance.FocusPosition(MapUtils.GetTileWorldPositionFromGridPosition(MapSystem.Instance.GetCityCenterPosition(cityGuid)));
    }

    private void OnTileYieldBonusSourceStart(Vector2Int sourcePosition)
    {
        CameraController.Instance.FocusPosition(MapUtils.GetTileWorldPositionFromGridPosition(sourcePosition));
        
        OnTileBonusYieldSourceStart?.Invoke(sourcePosition);
    }

    private void OnTileYieldBonusGranted(Vector2Int sourcePosition, Vector2Int destinationPosition, int yieldDiff)
    {
        TileVisuals tileInstanceAtPosition = MapVisualsController.Instance.GetTileInstanceAtPosition(destinationPosition);

        if (tileInstanceAtPosition != null)
        {
            tileInstanceAtPosition.TileYieldIncreasedAnimation(yieldDiff);
        }
    }

    private void OnTileYieldBonusSourceEnd(Vector2Int position)
    {
        OnTileBonusYieldSourceEnd?.Invoke(position);
    }

    private void OnRelicTriggered(RelicTypes relicType, Vector2Int source, Vector2Int destination, int yieldDiff)
    {
        if (yieldDiff != 0)
        {
            TryAnimateTile(source, destination, yieldDiff);
        }
    }

    private void TryAnimateTile(Vector2Int source, Vector2Int dest, int yieldDiff)
    {
        if (yieldDiff != 0)
        {
            TileVisuals tileInstanceAtPosition = MapVisualsController.Instance.GetTileInstanceAtPosition(dest);

            tileInstanceAtPosition.TriggerTileHarvestAnimation();

            OnTileBonusYieldApplied?.Invoke(source, dest, yieldDiff);
        }
    }
}
