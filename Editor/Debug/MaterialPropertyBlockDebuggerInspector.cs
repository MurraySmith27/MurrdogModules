#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MaterialPropertyBlockDebugger))]
public class MaterialPropertyBlockDebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        var dbg = (MaterialPropertyBlockDebugger)target;

        if (GUILayout.Button("Refresh Values"))
        {
            dbg.Refresh();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Property Values", EditorStyles.boldLabel);

        foreach (var v in dbg.values)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Name", v.name);
            EditorGUILayout.LabelField("Type", v.type);
            EditorGUILayout.LabelField("Value", v.value);
            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
