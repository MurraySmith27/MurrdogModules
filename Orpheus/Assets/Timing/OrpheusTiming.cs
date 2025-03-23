using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using System;

public class OrpheusTiming
{
    public static float WaitForSecondsGameTime(float waitTime)
    {
        if (float.IsNaN(waitTime)) waitTime = 0f;
        return Timing.LocalTime + ConvertToGameTimeSeconds(waitTime);
    }
    
    public static void InvokeCallbackAfterSecondsGameTime(float seconds, Action callback)
    {
        MEC.Timing.RunCoroutine(InvokeAfterSecondsGameTime(seconds, callback));
    }
    
    public static IEnumerator<float> InvokeAfterSecondsGameTime(float seconds, Action onEnd)
    {
        yield return WaitForSecondsGameTime(seconds);
        onEnd.Invoke();
    }

    public static float ConvertToGameTimeSeconds(float seconds)
    {
        return seconds / GlobalSettings.GameSpeed;
    }
}
