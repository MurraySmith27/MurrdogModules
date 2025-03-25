using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RelicIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RawImage rawImage;

    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private RelicVisualsSO relicVisuals;

    private RelicTypes _relicType;

    private int _currentRelicIndex;
        
    public void Populate(Rect renderTextureUV, RelicTypes relicTypes)
    {
        rawImage.uvRect = renderTextureUV;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _currentRelicIndex = TooltipManager.Instance.ShowTooltip(eventData.position, relicVisuals.GetDescriptionForRelic(_relicType));
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_currentRelicIndex != -1)
        {
            TooltipManager.Instance.HideTooltip(_currentRelicIndex);
        }

        _currentRelicIndex = -1;
    }
}
