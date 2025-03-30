using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddRandomTileToCityButton : MonoBehaviour
{
    [SerializeField] private Button button;

    private bool _isCurrentlySelectedTilePartOfCity = false;
    private Guid _currentlySelectedTileCityGuid;
    
    private void Start()
    {
        MapInteractionController.Instance.OnTileSelected -= TryToggleButtonActive;
        MapInteractionController.Instance.OnTileSelected += TryToggleButtonActive;
        button.interactable = false;
    }

    private void OnDestroy()
    {
        if (MapInteractionController.IsAvailable)
        {
            MapInteractionController.Instance.OnTileSelected -= TryToggleButtonActive;
        }
    }

    private void TryToggleButtonActive(TileVisuals tileVisuals, Vector2Int position)
    {
        _isCurrentlySelectedTilePartOfCity = MapSystem.Instance.IsTileOwnedByCity(position);
        button.interactable = _isCurrentlySelectedTilePartOfCity;
        MapSystem.Instance.GetCityGuidFromTile(position, out _currentlySelectedTileCityGuid);
    }

    public void AddRandomTileToCurrentlySelectedCity()
    {
        if (_isCurrentlySelectedTilePartOfCity)
        {
            MapSystem.Instance.AddRandomTileToCity(_currentlySelectedTileCityGuid);
        }   
    }
}
