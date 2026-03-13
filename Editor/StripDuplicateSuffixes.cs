using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class StripDuplicateSuffixes
{
    [MenuItem("Custom Tools/Strip Duplicate Suffixes in Selected Folder")]
    private static void Strip()
    {
        string folderPath = GetSelectedFolderPath();
        if (folderPath == null)
        {
            Debug.LogWarning("No folder selected in the Project window.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("", new[] { folderPath });
        int renamed = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Skip assets inside subfolders
            if (Path.GetDirectoryName(assetPath).Replace('\\', '/') != folderPath)
                continue;

            string oldName = Path.GetFileNameWithoutExtension(assetPath);
            string newName = Regex.Replace(oldName, @"\s*\(\d+\)$", "").TrimEnd();

            if (newName == oldName) continue;

            string error = AssetDatabase.RenameAsset(assetPath, newName);
            if (string.IsNullOrEmpty(error))
            {
                Debug.Log($"Renamed: \"{oldName}\" → \"{newName}\"");
                renamed++;
            }
            else
            {
                Debug.LogError($"Failed to rename \"{oldName}\": {error}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Done. {renamed} file(s) renamed in {folderPath}");
    }

    [MenuItem("Tools/Strip Duplicate Suffixes in Selected Folder", true)]
    private static bool ValidateStrip()
    {
        return GetSelectedFolderPath() != null;
    }

    private static string GetSelectedFolderPath()
    {
        Object selected = Selection.activeObject;
        if (selected == null) return null;

        string path = AssetDatabase.GetAssetPath(selected);
        if (AssetDatabase.IsValidFolder(path)) return path;

        return null;
    }
}
