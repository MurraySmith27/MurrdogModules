using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine;

public static class ToggleSelectedGameObject
{
    // Ctrl/Cmd + Shift + T
    [MenuItem("Tools/Toggle Selected GameObject %#t")]
    private static void Toggle()
    {
        if (Selection.activeGameObject != null)
        {
            GameObject go = Selection.activeGameObject;

            Undo.RecordObject(go, "Toggle Active State");
            go.SetActive(!go.activeSelf);
            EditorUtility.SetDirty(go);
        }
        else
        {
            Debug.LogWarning("No GameObject selected.");
        }
    }

    // Enables/disables menu item based on selection
    [MenuItem("Tools/Toggle Selected GameObject %#t", true)]
    private static bool ValidateToggle()
    {
        return Selection.activeGameObject != null;
    }
}
