using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMouseController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private OrpheusUIInputChannel inputChannel;

    private void Start()
    {
        inputChannel.MouseDownEvent -= OnMouseDown;
        inputChannel.MouseDownEvent += OnMouseDown;
    }

    private void OnDestroy()
    {
        inputChannel.MouseDownEvent -= OnMouseDown;
    }

    private void OnMouseDown(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        if (MapInteractionController.IsAvailable)
        {
            if (args.vector2Arg.HasValue)
            {
                Vector2 mousePos = args.vector2Arg.Value;
                Vector3 screenSpacePos = new Vector3(mousePos.x, mousePos.y, 10f);

                Vector3 directionFromEye =
                    (mainCamera.ScreenToWorldPoint(screenSpacePos) - mainCamera.transform.position).normalized;

                float t = -mainCamera.transform.position.y / directionFromEye.y;

                Vector3 pointOnGroundPlane = directionFromEye * t + mainCamera.transform.position;

                Vector2Int tilePos = new Vector2Int(
                    Mathf.FloorToInt(pointOnGroundPlane.x / GameConstants.TILE_SIZE),
                    Mathf.FloorToInt(pointOnGroundPlane.z / GameConstants.TILE_SIZE));
                MapInteractionController.Instance.SelectTile(tilePos);
            }
        }
    }
}
