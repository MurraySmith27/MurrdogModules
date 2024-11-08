using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{
    [MenuItem("Tools/Clear PlayerPrefs", true)]
    public static bool DoClearPlayerPrefs_Validate()
    {
        return true;
    }
    
    [MenuItem("Custom Tools/Clear PlayerPrefs", false)]
    public static void DoClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
