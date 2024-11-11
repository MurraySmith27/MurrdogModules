using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AsyncUtils
{

    private delegate bool WaitUntilConditionDelegate();
    
    public static IEnumerator InvokeAfterSeconds(float seconds, Action onEnd)
    {
        yield return new WaitForSeconds(seconds);
        onEnd.Invoke();
    }
    
    public static IEnumerator InvokeAfterSecondsRealtime(float seconds, Action onEnd)
    {
        yield return new WaitForSecondsRealtime(seconds);
        onEnd.Invoke();
    }
    
    private static IEnumerator InvokeAfterCondition(Action onEnd, WaitUntilConditionDelegate condition)
    {
        yield return new WaitUntil(() => condition.Invoke());
        onEnd.Invoke();
    }
}
