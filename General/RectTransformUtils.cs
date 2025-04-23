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
        var corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        float xComponent = Mathf.Lerp(corners[0].x, corners[3].x,
            (preferredDirection.normalized.x + 1f) / 2f);
        float yComponent = Mathf.Lerp(corners[0].y, corners[1].y,
            (preferredDirection.normalized.y + 1f) / 2f);
        
        Vector4 worldPos = rectTransform.localToWorldMatrix * new Vector2(xComponent, yComponent);

        return offset + new Vector2(worldPos.x, worldPos.y);
    }
}
