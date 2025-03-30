using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelectedIndicator : MonoBehaviour
{
    private void Start()
    {
        MapInteractionController.Instance.OnTileSelected -= OnTileSelected;
        MapInteractionController.Instance.OnTileSelected += OnTileSelected;

        MapInteractionController.Instance.OnMapInteractionModeChanged -= OnMapInteractionModeChanged;
        MapInteractionController.Instance.OnMapInteractionModeChanged += OnMapInteractionModeChanged;
    }

    private void OnDestroy()
    {
        if (MapInteractionController.IsAvailable)
        {
            MapInteractionController.Instance.OnTileSelected -= OnTileSelected;
            MapInteractionController.Instance.OnMapInteractionModeChanged -= OnMapInteractionModeChanged;
        }
    }

    private void OnTileSelected(TileVisuals tile, Vector2Int tilePosition)
    {
        transform.position = tile.transform.position;
    }

    private void OnMapInteractionModeChanged(MapInteractionMode mode)
    {
        if (mode != MapInteractionMode.Default)
        {
            transform.position = new Vector3(0, -100, 0);
        }
    }
}
