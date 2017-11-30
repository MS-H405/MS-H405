using System.Collections;
using UnityEngine;

public sealed class WaitForSecondsRealtime : CustomYieldInstruction
{
    private float totalTime = 0.0f;
    private float waitTime;

    public override bool keepWaiting
    {
        get {
            totalTime += Time.deltaTime;
            //Debug.Log("TotalTime" + totalTime);
            return totalTime < waitTime;
        }
    }

    public WaitForSecondsRealtime(float time)
    {
        waitTime = totalTime + time;
    }
}