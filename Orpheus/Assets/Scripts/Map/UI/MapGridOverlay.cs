using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGridOverlay : MonoBehaviour
{
    private void Start()
    {
        TileFrustrumCulling.Instance.OnTileCullingUpdated -= OnTileCullingUpdated;
        TileFrustrumCulling.Instance.OnTileCullingUpdated += OnTileCullingUpdated;
    }

    private void OnDestroy()
    {
        if (TileFrustrumCulling.IsAvailable)
        {
            TileFrustrumCulling.Instance.OnTileCullingUpdated -= OnTileCullingUpdated;
        }
    }

    private void OnTileCullingUpdated(int x, int y, int width, int height)
    {
        Vector2Int centerPos = new Vector2Int(Mathf.RoundToInt(x + width / 2f), Mathf.RoundToInt(y + height / 2f));

        transform.position = MapUtils.GetTileWorldPositionFromGridPosition(centerPos);
    }
}
