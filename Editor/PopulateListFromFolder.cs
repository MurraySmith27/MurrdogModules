using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(PopulateFromFolderList))]
public class PopulateFromFolderDrawer : PropertyDrawer
{
    private readonly Dictionary<string, ReorderableList> _lists = new();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return GetOrBuildList(property, label).GetHeight();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GetOrBuildList(property, label).DoList(position);
    }

    private ReorderableList GetOrBuildList(SerializedProperty property, GUIContent label)
    {
        string key = property.propertyPath;

        if (_lists.TryGetValue(key, out var existing))
        {
            existing.serializedProperty = property.FindPropertyRelative("_items");
            return existing;
        }

        SerializedProperty itemsProp = property.FindPropertyRelative("_items");
        var list = new ReorderableList(property.serializedObject, itemsProp, true, true, true, true);

        list.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, label);

            if (Event.current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition))
            {
                var so = list.serializedProperty.serializedObject;
                var path = list.serializedProperty.propertyPath;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Populate from Folder..."), false,
                    () => PopulateListFromFolderWindow.Show(so, path));
                menu.ShowAsContext();
                Event.current.Use();
            }
        };

        list.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.ObjectField(rect, element, typeof(GameObject), GUIContent.none);
        };

        _lists[key] = list;
        return list;
    }
}

public class PopulateListFromFolderWindow : EditorWindow
{
    private SerializedObject _serializedObject;
    private string _propertyPath;
    private string _folderPath = "";
    private bool _recursive;

    public static void Show(SerializedObject serializedObject, string propertyPath)
    {
        var window = CreateInstance<PopulateListFromFolderWindow>();
        window.titleContent = new GUIContent("Populate List from Folder");
        window._serializedObject = serializedObject;
        window._propertyPath = propertyPath;
        window._folderPath = NormalizeToAssetPath(GUIUtility.systemCopyBuffer);
        window.minSize = new Vector2(450, 110);
        window.maxSize = new Vector2(450, 110);
        window.ShowUtility();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();
        _folderPath = EditorGUILayout.TextField("Folder", _folderPath);
        if (GUILayout.Button("Browse", GUILayout.Width(60)))
        {
            string picked = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
            if (!string.IsNullOrEmpty(picked))
                _folderPath = NormalizeToAssetPath(picked);
        }
        EditorGUILayout.EndHorizontal();

        _recursive = EditorGUILayout.Toggle("Include Subfolders", _recursive);

        EditorGUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Populate"))
        {
            if (Populate()) Close();
        }
        if (GUILayout.Button("Cancel"))
            Close();
        EditorGUILayout.EndHorizontal();
    }

    private bool Populate()
    {
        if (!AssetDatabase.IsValidFolder(_folderPath))
        {
            EditorUtility.DisplayDialog("Invalid Folder",
                $"\"{_folderPath}\" is not a valid folder in the Asset database.", "OK");
            return false;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { _folderPath });
        var found = new List<GameObject>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!_recursive && Path.GetDirectoryName(path).Replace('\\', '/') != _folderPath)
                continue;

            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go != null) found.Add(go);
        }

        _serializedObject.Update();
        SerializedProperty prop = _serializedObject.FindProperty(_propertyPath);
        prop.arraySize = found.Count;
        for (int i = 0; i < found.Count; i++)
            prop.GetArrayElementAtIndex(i).objectReferenceValue = found[i];
        _serializedObject.ApplyModifiedProperties();

        Debug.Log($"Populated {found.Count} prefab(s) from \"{_folderPath}\".");
        return true;
    }

    private static string NormalizeToAssetPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        path = path.Trim().Replace('\\', '/').TrimEnd('/');

        string dataPath = Application.dataPath.Replace('\\', '/').TrimEnd('/');
        if (path.StartsWith(dataPath))
            return "Assets" + path.Substring(dataPath.Length);

        if (path.StartsWith("Assets/") || path == "Assets")
            return path;

        return path;
    }
}
