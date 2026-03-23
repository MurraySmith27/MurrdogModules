using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using System.Linq;

public class ConvertSkinnedMeshes
{
    [MenuItem("Tools/Convert SkinnedMeshRenderers In Folder")]
    public static void ConvertFolder()
    {
        string folderPath = "Assets/Prefabs/Resources/Wood/TreePrefabs"; // CHANGE THIS

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        int convertedCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
            bool modified = false;

            var skinnedMeshes = prefabRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            foreach (var smr in skinnedMeshes)
            {
                GameObject go = smr.gameObject;

                // Skip if no mesh
                if (smr.sharedMesh == null)
                    continue;

                // Save properties
                var mesh = smr.sharedMesh;
                var materials = smr.sharedMaterials;

                var shadowCasting = smr.shadowCastingMode;
                var receiveShadows = smr.receiveShadows;
                var motionVectors = smr.motionVectorGenerationMode;
                var lightProbeUsage = smr.lightProbeUsage;
                var reflectionProbeUsage = smr.reflectionProbeUsage;

                var probeAnchor = smr.probeAnchor;
                var rootBone = smr.rootBone; // (not used but kept for reference)

                var sortingLayerID = smr.sortingLayerID;
                var sortingOrder = smr.sortingOrder;

                // Remove SkinnedMeshRenderer
                Object.DestroyImmediate(smr, true);

                // Add MeshFilter + MeshRenderer
                var mf = go.GetComponent<MeshFilter>();
                if (mf == null)
                    mf = go.AddComponent<MeshFilter>();

                mf.sharedMesh = mesh;

                var mr = go.GetComponent<MeshRenderer>();
                if (mr == null)
                    mr = go.AddComponent<MeshRenderer>();

                // Restore properties
                mr.sharedMaterials = materials;
                mr.shadowCastingMode = shadowCasting;
                mr.receiveShadows = receiveShadows;
                mr.motionVectorGenerationMode = motionVectors;
                mr.lightProbeUsage = lightProbeUsage;
                mr.reflectionProbeUsage = reflectionProbeUsage;

                mr.probeAnchor = probeAnchor;

                mr.sortingLayerID = sortingLayerID;
                mr.sortingOrder = sortingOrder;

                modified = true;
                convertedCount++;
            }

            if (modified)
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
            }

            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        Debug.Log($"Converted {convertedCount} SkinnedMeshRenderers.");
    }
}