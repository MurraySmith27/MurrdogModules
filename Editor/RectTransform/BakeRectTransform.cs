#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BakeRectTransform
{
    [MenuItem("Custom Tools/Bake RectTransform position to anchors", true)]
    public static bool BakeRectTransformPositionToAnchors_Validate()
    {
        return Selection.activeGameObject != null &&
               Selection.activeGameObject.GetComponent<RectTransform>() != null;
    }
    
    [MenuItem("Custom Tools/Bake RectTransform position to anchors", false)]
    public static void BakeRectTransformPositionToAnchors()
    {
        RectTransform rt = Selection.activeGameObject.GetComponent<RectTransform>();

        if (rt != null)
        {
            RectTransform parent = rt.parent.GetComponent<RectTransform>();

            if (parent != null)
            {
                Undo.RecordObject(rt, "Bake Rect Transform to Anchor Positions Using Editor Tool");

                rt.anchorMin = (rt.anchorMin * parent.rect.size + rt.offsetMin) / parent.rect.size;
                rt.anchorMax = (rt.anchorMax * parent.rect.size + rt.offsetMax) / parent.rect.size;

                rt.sizeDelta = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;
            }
        }
    }
}

#endif
