using System;
using UnityEditor;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T: Singleton<T>
{
    protected static T _instance;

    protected static bool quitting;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
            }

            if (_instance == null)
            {
                _instance = Create();
            }
            
            return _instance;
        }
    }

    protected static T Create()
    {
        var obj = new GameObject(typeof(T).ToString());
        // DontDestroyOnLoad(obj);
        return obj.AddComponent<T>();
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
        }
        
        if(_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }

    protected void OnDestroy()
    {
        if (_instance == this)
        {
            quitting = true;
        }
    }
}