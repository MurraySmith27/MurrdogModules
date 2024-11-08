using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;

public static class DeleteBuildStreamingAssetsIfUnnecessary
{

    public static void OnPreprocessBuild()
    {
        if (!Debug.isDebugBuild)
            AssetDatabase.MoveAsset("Assets/Resources/UserProfile/", "Assets/UserProfile/");
    }

    [PostProcessBuild(100)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (!Debug.isDebugBuild)
        {
            // Restore assets to Resources
            AssetDatabase.MoveAsset("Assets/UserProfile/", "Assets/Resources/UserProfile/");
            // I don't know if this is necessary
            AssetDatabase.DeleteAsset("Assets/UserProfile");

            // Refresh database
            AssetDatabase.Refresh();
        }
    }
}