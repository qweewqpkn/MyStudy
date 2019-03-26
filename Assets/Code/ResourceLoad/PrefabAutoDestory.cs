using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabAutoDestroy : MonoBehaviour {

    public HRes mRes;
    void OnDestroy()
    {
        if(mRes != null)
        {
            mRes.Release();
        }
    }

    //外部实例化带有PrefabAutoDestroy的对象，需要调用这个借口增加引用
    public void AddRef()
    {
        if(mRes != null)
        {
            mRes.AddRef();
        }
    }
}
