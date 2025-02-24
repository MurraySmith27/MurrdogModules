using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHoveredOverIndicator : MonoBehaviour
{
    private void Start()
    {
        MapInteractionController.Instance.OnTileHoveredOver -= OnTileHoveredOver;
        MapInteractionController.Instance.OnTileHoveredOver += OnTileHoveredOver;
    }

    private void OnDestroy()
    {
        if (MapInteractionController.IsAvailable)
        {
            MapInteractionController.Instance.OnTileSelected -= OnTileHoveredOver;
        }
    }

    private void OnTileHoveredOver(TileVisuals tile)
    {
        transform.position = tile.transform.position;
    }
}
