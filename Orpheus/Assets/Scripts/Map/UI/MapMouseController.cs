using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMouseController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private OrpheusUIInputChannel inputChannel;

    private void Start()
    {
        inputChannel.LeftMouseClickEvent -= OnLeftMouseDown;
        inputChannel.LeftMouseClickEvent += OnLeftMouseDown;

        inputChannel.MouseMoveEvent -= OnMouseMove;
        inputChannel.MouseMoveEvent += OnMouseMove;
    }

    private void OnDestroy()
    {
        inputChannel.LeftMouseClickEvent -= OnLeftMouseDown;
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
                Vector2 mousePos = args.vector2Arg.Value;

                if (mousePos.x >= 0 && mousePos.x <= Screen.width && mousePos.y >= 0 && mousePos.y <= Screen.height)
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
}
