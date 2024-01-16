using UnityEditor;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T: Singleton<T>
{
    static T _instance;

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
                _instance = CreateGameObject();
            }
            
            return _instance;
        }
    }

    protected static T CreateGameObject()
    {
        var obj = new GameObject(typeof(T).ToString());
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
            DestroyImmediate(this);
        }
        
                
    }
}