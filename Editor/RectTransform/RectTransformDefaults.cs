using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class RectTransformDefaults
{
    static RectTransformDefaults()
    {
        ObjectFactory.componentWasAdded += OnComponentWasAdded;
    }

    private static void OnComponentWasAdded(Component c)
    {
        EditorApplication.delayCall += () =>
        {
            if (c is RectTransform rt)
            {
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                EditorUtility.SetDirty(rt);
            }
        };
    }
}