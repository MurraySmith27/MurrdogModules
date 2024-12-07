using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AsyncUtils
{

    public delegate bool WaitUntilConditionDelegate();
    
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
    
    public static IEnumerator InvokeAfterCondition(Action onEnd, WaitUntilConditionDelegate condition)
    {
        yield return new WaitUntil(() => condition.Invoke());
        onEnd.Invoke();
    }
    
    public class WaitForSecondsOrUntil : CustomYieldInstruction
    {
        private float t = 0f;
        private float waitSeconds;
        private Func<bool> waitCondition;
        
        public override bool keepWaiting
        {
            get
            {
                t += Time.deltaTime;
                return t <= waitSeconds && !waitCondition();
            }
        }

        public WaitForSecondsOrUntil(float seconds, Func<bool> condition)
        {
            t = 0f;
            waitSeconds = seconds;
            waitCondition = condition;
        }
    }
}
