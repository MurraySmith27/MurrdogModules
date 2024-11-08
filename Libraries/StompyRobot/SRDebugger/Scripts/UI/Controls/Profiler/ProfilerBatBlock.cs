using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilerBatBlock : MonoBehaviour
{
    public GameObject batContainer;
    public Text batLevel;
    public Text usage15;
    public Text usage30;
    public Text usage60;
    public Text usage90;

    private float lastUpdate;
    private LinkedList<(float,float)> updateQueue = new LinkedList<(float,float)>();

    private void Awake()
    {
        if (SystemInfo.batteryStatus == BatteryStatus.Unknown) 
            batContainer.SetActive(false);
        lastUpdate = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        if (SystemInfo.batteryStatus == BatteryStatus.Unknown) return;   
        if ( Time.realtimeSinceStartup - lastUpdate > 10f)
        {
            var nowTime = Time.realtimeSinceStartup;
            updateQueue.AddLast((nowTime, SystemInfo.batteryLevel));
            if (updateQueue.Count > 540) updateQueue.RemoveFirst();

            var sum15 = 0f;
            var sum30 = 0f;
            var sum60 = 0f;
            var sum90 = 0f;

            float previousLevel = -1f;
            foreach (var (timeRecorded, batteryLevel) in updateQueue)
            {
                var timeDiff = nowTime - timeRecorded;
                var levelDiff = previousLevel - batteryLevel;

                if (previousLevel == -1f)
                {
                    previousLevel = batteryLevel;
                    continue;
                }

                previousLevel = batteryLevel;

                if (timeDiff <= 15 * 60)
                    sum15 += levelDiff;
                if (timeDiff <= 30 * 60)
                    sum30 += levelDiff;
                if (timeDiff <= 60 * 60)
                    sum60 += levelDiff;
                if (timeDiff <= 90 * 60)
                    sum90 += levelDiff;
            }

            batLevel.text = $"Battery Level: {SystemInfo.batteryLevel * 100f}%";
            usage15.text = $"Battery Usage(15m): {Mathf.Ceil(sum15 * 100f)}%";
            usage30.text = $"Battery Usage(30m): {Mathf.Ceil(sum30 * 100f)}%";
            usage60.text = $"Battery Usage(60m): {Mathf.Ceil(sum60 * 100f)}%";
            usage90.text = $"Battery Usage(90m): {Mathf.Ceil(sum90 * 100f)}%";
            lastUpdate = nowTime;
        }
    }
}
