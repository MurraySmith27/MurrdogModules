using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class RectTransformUtils
{

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
}
