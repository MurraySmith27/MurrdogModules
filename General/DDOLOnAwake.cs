using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOLOnAwake : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
