using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class GlobalSettings
{
    public static bool IsMapDraggingEnabled
    {
        get
        {
            return PlayerPrefs.GetInt(nameof(IsMapDraggingEnabled), 1) != 0;
        }
        set
        {
            PlayerPrefs.SetInt(nameof(IsMapDraggingEnabled), value ? 1 : 0);
        }
    }
}
