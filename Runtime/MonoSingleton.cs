using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError(typeof(T).ToString() + "is null");

            return _instance;
        }
    }

    public void Awake()
    {
        if (!Application.isPlaying)
            return;

        _instance = this as T;
        DontDestroyOnLoad(this);
    }

    public static bool InstanceExists
    {
        get
        {
            return _instance != null;
        }
    }
}