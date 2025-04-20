using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipBase : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    
    [SerializeField] private OrpheusUIInputChannel uiInputChannel;
    
    private int _currentTooltipIndex = -1;
    private void Update()
    {
        bool isPointerOver = UIMouseData.Instance.IsMouseOverRectTransform(rectTransform);

        if (isPointerOver && _currentTooltipIndex == -1)
        {
            Vector2 worldPosition = rectTransform.TransformPoint(rectTransform.rect.center);
            _currentTooltipIndex =
                TooltipManager.Instance.ShowTooltip(worldPosition, GetTooltipText(),
                    () => { _currentTooltipIndex = -1; });
        }
        else if (!isPointerOver && _currentTooltipIndex != -1)
        {
            TooltipManager.Instance.HideTooltipIfMouseOff(_currentTooltipIndex);
        }
    }

    private void OnDisable()
    {
        if (_currentTooltipIndex != -1)
        {
            TooltipManager.Instance.HideTooltipIfMouseOff(_currentTooltipIndex);
        }
    }
    
    private void OnDestroy()
    {
        if (_currentTooltipIndex != -1)
        {
            TooltipManager.Instance.HideTooltipIfMouseOff(_currentTooltipIndex);
        }
    }

    protected virtual string GetTooltipText()
    {
        return "";
    }
}
