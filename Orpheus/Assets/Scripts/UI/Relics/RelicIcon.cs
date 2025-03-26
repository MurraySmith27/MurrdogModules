using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RelicIcon : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;

    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private RelicVisualsSO relicVisuals;

    [SerializeField] private OrpheusUIInputChannel uiInputChannel;

    private RelicTypes _relicType;

    private int _currentRelicIndex = -1;

    private bool _pointerOver;

    private void Start()
    {
        uiInputChannel.MouseMoveEvent -= OnMouseMove;
        uiInputChannel.MouseMoveEvent += OnMouseMove;
    }

    private void OnDestroy()
    {
        if (uiInputChannel != null)
        {
            uiInputChannel.MouseMoveEvent -= OnMouseMove;
        }
    }

    public void Populate(Rect renderTextureUV, RelicTypes relicTypes)
    {
        rawImage.uvRect = renderTextureUV;
        _relicType = relicTypes;
    }

    public void OnMouseMove(UIInputChannel.UIInputChannelCallbackArgs args)
    {
        bool isPointerOver = RectTransformUtils.IsMouseOverRectTransform(rectTransform);
        Debug.LogError($"ispointerover: {isPointerOver}");
        if (isPointerOver && !_pointerOver && _currentRelicIndex == -1)
        {
            _pointerOver = true;
            
            Vector2 worldPosition = rectTransform.TransformPoint(rectTransform.rect.center);
            _currentRelicIndex =
                TooltipManager.Instance.ShowTooltip(worldPosition, relicVisuals.GetDescriptionForRelic(_relicType));
        }
        else if (_pointerOver && !isPointerOver && _currentRelicIndex != -1)
        {
            _pointerOver = false;
            
            TryHideTooltip();
        }
    }

    private void TryHideTooltip()
    {
        if (TooltipManager.Instance.HideTooltipIfMouseOff(_currentRelicIndex))
        {
            _currentRelicIndex = -1;
        }
    }
}
