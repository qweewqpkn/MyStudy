using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISingleton
{
    void Init();
}

public class Singleton<T> : ISingleton where T : ISingleton, new() {
    private static T mInstance;

    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new T();
                mInstance.Init();
            }

            return mInstance;
        }
    }

    public virtual void Init()
    {

    }
}
