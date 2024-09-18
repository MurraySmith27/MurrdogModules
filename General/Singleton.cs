using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public bool DDOL = true;
    
    private bool _initRan = false;
    private static T _instance;

    private static object _lock = new object();

    public static bool IsAvailable
    {
        get
        {
            return _instance != null;
        }
    }
    
    private void Awake() { _init(); OnAwake(); }

    public virtual void OnAwake()
    {
        
    }
    
    private void OnEnable() { _init(); }
    
    private void Start() { _init(); }
    
    private void _init()
    {
        if (_initRan) return;

        if (_instance == null)
        {
            _instance = GetComponent<T>();
        }
        else if (gameObject != _instance.gameObject)
        {
            enabled = false;
        }

        if (enabled && DDOL)
        {
            DontDestroyOnLoad(gameObject);
        }

        _initRan = true;
    }

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>(true);

                    if (_instance == null)
                    {
                        string typeName = typeof(T).Name;
#if UNITY_EDITOR
                        UnityEditor.EditorUtility.DisplayDialog("Singleton Error",
                            $"Tried to access singleton of {typeName} before it was initialized.", "OK");
#endif
                        
                        throw new FieldAccessException(
                            $"Tried to access Singleton instance of {typeName} before it was initialized.");
                    }
                }

                return _instance;
            }
        }
    }
}
