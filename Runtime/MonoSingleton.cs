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
        if (_instance == null)
        {
            _instance = this as T;
            // Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (_instance != this)
        {
            // If there's any other object exist of this type delete it
            // as it's breaking our singleton pattern
            Destroy(gameObject);
        }
    }

    public static bool InstanceExists
    {
        get
        {
            return _instance != null;
        }
    }
}