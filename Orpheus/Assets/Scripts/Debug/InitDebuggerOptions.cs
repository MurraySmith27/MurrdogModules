using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitDebuggerOptions : MonoBehaviour
{
    #if DEVELOPMENT_BUILD || UNITY_EDITOR
    void Awake()
    {
        SRDebug.Instance.AddOptionContainer(DebugOptions.Instance);
    }
    #endif   
}
