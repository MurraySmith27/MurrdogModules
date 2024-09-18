using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BakeScaleToChildren : EditorWindow
{

    public Object source;
    
    [MenuItem("Custom Tools/Bake Scale To Children")]
    public static void ShowBakeScaleToChildrenWindow()
    {
        BakeScaleToChildren window = GetWindowWithRect<BakeScaleToChildren>(new Rect(0, 0, 165, 100));

        window.titleContent = new GUIContent("Bake Scale To Children");

        window.Show();
    }
    
    public void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Parent GameObject:");
        source = EditorGUILayout.ObjectField(source, typeof(Object), true);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Bake Scale To Children"))
        {
            if (source == null)
                ShowNotification(new GUIContent("No object selected!"));
            else
                OnBakeScaleToChildren((GameObject)source);
        }
    }
    
    private void OnBakeScaleToChildren(GameObject parent)
    {
        Vector3 parentScale = parent.transform.localScale;
        
        Undo.RecordObject(parent.transform, "Set Parent Scale to 1");
        parent.transform.localScale = new Vector3(1, 1, 1);
        
        for (int childNum = 0; childNum < parent.transform.childCount; childNum++)
        {
            Transform child = parent.transform.GetChild(childNum);
            
            Undo.RecordObject(child.transform, "Modify Local Scale");
            child.transform.localScale = new Vector3(child.transform.localScale.x * parentScale.x,
                child.transform.localScale.y * parentScale.y, child.transform.localScale.z * parentScale.z);
        }

    }
}
