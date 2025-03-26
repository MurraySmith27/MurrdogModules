using System;
using UnityEngine;

public static class GlobalSettings
{
    public static bool IsMapDraggingEnabled
    {
        get { return PlayerPrefs.GetInt(nameof(IsMapDraggingEnabled), 1) != 0; }
        set { PlayerPrefs.SetInt(nameof(IsMapDraggingEnabled), value ? 1 : 0); }
    }


    public static event Action<float> OnGameSpeedChanged;
    
    public static float GameSpeed
    {
        get { return PlayerPrefs.GetFloat(nameof(GameSpeed), 1f); }
        set
        {
            PlayerPrefs.SetFloat(nameof(GameSpeed), value);
            
            OnGameSpeedChanged?.Invoke(value);
        }
    }

    public static float TextSizeMultiplier
    {
        get
        {
            return PlayerPrefs.GetFloat(nameof(TextSizeMultiplier), 1f);
        }
        set
        {
            PlayerPrefs.SetFloat(nameof(TextSizeMultiplier), value);
        }
    }

}
