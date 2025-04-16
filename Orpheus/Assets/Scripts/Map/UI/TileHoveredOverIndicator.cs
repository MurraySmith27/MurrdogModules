using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHoveredOverIndicator : MonoBehaviour
{
    private void Start()
    {
        MapInteractionController.Instance.OnTileHoveredOver -= OnTileHoveredOver;
        MapInteractionController.Instance.OnTileHoveredOver += OnTileHoveredOver;

        MapInteractionController.Instance.OnMapInteractionModeChanged -= OnMapInteractionModeChanged;
        MapInteractionController.Instance.OnMapInteractionModeChanged += OnMapInteractionModeChanged;
    }

    private void OnDestroy()
    {
        if (MapInteractionController.IsAvailable)
        {
            MapInteractionController.Instance.OnTileSelected -= OnTileHoveredOver;
            MapInteractionController.Instance.OnMapInteractionModeChanged -= OnMapInteractionModeChanged;
        }
    }

    private void OnTileHoveredOver(TileVisuals tile, Vector2Int position)
    {
        transform.position = tile.transform.position;
    }

    private void OnMapInteractionModeChanged(MapInteractionMode mapInteractionMode)
    {
        transform.position = new Vector3(0, -100, 0);
    }
}
