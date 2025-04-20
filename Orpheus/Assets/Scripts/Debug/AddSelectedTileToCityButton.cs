using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddSelectedTileToCityButton : MonoBehaviour
{
    [SerializeField] private Button button;
    
    private void Start()
    {
        MapInteractionController.Instance.OnTileSelected -= OnTileSelected;
        MapInteractionController.Instance.OnTileSelected += OnTileSelected;
        
        button.interactable = false;
    }

    private void OnDestroy()
    {
        if (MapInteractionController.IsAvailable)
        {
            MapInteractionController.Instance.OnTileSelected -= OnTileSelected;
        }
    }

    private void OnTileSelected(TileVisuals tileVisuals, Vector2Int position)
    {
        button.interactable = !MapSystem.Instance.IsTileOwnedByCity(position);
    }
    
    public void AddSelectedTileToCity()
    {
        Vector2Int currentlySelectedTile = MapInteractionController.Instance.GetCurrentlySelectedTile();

        if (!MapSystem.Instance.IsTileOwnedByCity(currentlySelectedTile))
        {
            List<Guid> cityGuids = MapSystem.Instance.GetAllCityGuids();

            if (cityGuids.Count != 0)
            {
                MapSystem.Instance.AddTileToCity(cityGuids[0], currentlySelectedTile, true);
            }
        }
    }
}
