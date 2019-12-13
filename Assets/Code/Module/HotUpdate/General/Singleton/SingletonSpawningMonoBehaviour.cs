/*-------------------------------------------------------------------
// Copyright (C)
//
// Module: SingletonSpawningMonoBehaviour
// Author: huangxin
// Date: 2017.09.07
// Description: MonoBehaviour Singleton, 不需要手动绑定GameObject.
//-----------------------------------------------------------------*/

using System;
using UnityEngine;

public class SingletonSpawningMonoBehaviour<T> : MonoBehaviour where T : SingletonSpawningMonoBehaviour<T>
{
    protected static bool applicationQuitting;
    private static T uniqueInstance;

    protected virtual void Awake()
    {
        if (uniqueInstance == null)
            uniqueInstance = (T)this;
        else if (uniqueInstance != null)
            throw new InvalidOperationException("Cannot have two instance of a MonoBehaviour Singleton : " + typeof(T).ToString() + ".");
        UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
		Exists = true;
    }

    protected virtual void OnApplicationQuit()
    {
        applicationQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        if (uniqueInstance == this)
        {
            Exists = false;
            uniqueInstance = null;
        }
    }

    public static bool Exists
    {
        get;
        private set;
    }

    public static T Instance
    {
        get
        {
            if (!Exists)
            {
                if (applicationQuitting || !Application.isPlaying)
                    return null;
                Type[] components = new Type[] { typeof(T) };
                GameObject target = new GameObject("Singleton " + typeof(T).ToString(), components);
                uniqueInstance = target.GetComponent<T>();
                Exists = true;
            }
            return uniqueInstance;
        }
    }
}
