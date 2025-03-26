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

    public void Populate(Rect renderTextureUV, RelicTypes relicTypes)
    {
        rawImage.uvRect = renderTextureUV;
        _relicType = relicTypes;
    }

    private void Update()
    {
        bool isPointerOver = UIMouseData.Instance.IsMouseOverRectTransform(rectTransform);
        
        if (isPointerOver && _currentRelicIndex == -1)
        {
            Vector2 worldPosition = rectTransform.TransformPoint(rectTransform.rect.center);
            _currentRelicIndex =
                TooltipManager.Instance.ShowTooltip(worldPosition, relicVisuals.GetDescriptionForRelic(_relicType),
                    () =>
                    {
                        _currentRelicIndex = -1;
                    });
        }
        else if (!isPointerOver && _currentRelicIndex != -1)
        {
            TooltipManager.Instance.HideTooltipIfMouseOff(_currentRelicIndex);
        }
    }
}
