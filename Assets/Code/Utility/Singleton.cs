using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : new() {
    private static T mInstance;

    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new T();
            }

            return mInstance;
        }
    }
}

public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
    private static T mInstance;

    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject obj = new GameObject(typeof(T).Name);
                mInstance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
                mInstance.Init();
            }

            return mInstance;
        }
    }

    protected virtual void Init()
    {

    }
}
