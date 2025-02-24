using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMouseController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private OrpheusUIInputChannel inputChannel;

    private void Start()
    {
        inputChannel.LeftMouseDownEvent -= OnLeftMouseDown;
        inputChannel.LeftMouseDownEvent += OnLeftMouseDown;

        inputChannel.MouseMoveEvent -= OnMouseMove;
        inputChannel.MouseMoveEvent += OnMouseMove;
    }

    private void OnDestroy()
    {
        inputChannel.LeftMouseDownEvent -= OnLeftMouseDown;
        inputChannel.MouseMoveEvent -= OnMouseMove;
    }

    private void OnLeftMouseDown(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        if (MapInteractionController.IsAvailable)
        {
            if (args.vector2Arg.HasValue)
            {
                Vector2Int tilePos;
                if (CameraUtils.GetTilePositionFromMousePosition(args.vector2Arg.Value, mainCamera, out tilePos))
                {
                    MapInteractionController.Instance.SelectTile(tilePos);
                }
            }
        }
    }

    private void OnMouseMove(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        if (MapInteractionController.IsAvailable)
        {
            if (args.vector2Arg.HasValue)
            {
                Vector2Int tilePos;
                if (CameraUtils.GetTilePositionFromMousePosition(args.vector2Arg.Value, mainCamera,
                        out tilePos))
                {
                    MapInteractionController.Instance.HoverOverTile(tilePos);
                }
            }
        }
    }
}
