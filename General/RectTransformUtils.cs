using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class RectTransformUtils
{

    public static List<RectTransform> GetMouseOverRectTransforms()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        
        return raycastResults.Select((RaycastResult result) => { return result.gameObject.transform as RectTransform; }).ToList();
    }
    
    public static bool IsMouseOverRectTransform(RectTransform rectTransform)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        
        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.transform == rectTransform)
            {
                return true;
            }
        }

        return false;
    }

    public static Vector2 GetPositionOutsideRectTransform(RectTransform rectTransform, Vector2 preferredDirection, Vector2 offset)
    {
        Vector2 topLeft = rectTransform.localToWorldMatrix * new Vector2(rectTransform.rect.xMin, rectTransform.rect.yMin);
        Vector2 topRight = rectTransform.localToWorldMatrix * new Vector2(rectTransform.rect.xMax, rectTransform.rect.yMin);
        Vector2 bottomLeft = rectTransform.localToWorldMatrix * new Vector2(rectTransform.rect.xMin, rectTransform.rect.yMax);
        Vector2 bottomRight = rectTransform.localToWorldMatrix * new Vector2(rectTransform.rect.xMax, rectTransform.rect.yMax);
        
        Vector2 position = new Vector2(Mathf.Lerp(rectTransform.rect.xMin, rectTransform.rect.xMax, ))
    }
}
