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
}
