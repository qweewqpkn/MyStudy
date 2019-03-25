using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabAutoDestory : MonoBehaviour {

    public HRes mRes;
    void OnDestroy()
    {
        if(mRes != null)
        {
            mRes.Release();
        }
    }

    //需要用在这种情况：当你从底层资源管理实例化了一个GameObject对象后，然后你在外部使用了缓存池，那么缓存池里面使用GameObject.Instantiate()后,
    //那么你就要在产生新对象的调用这个方法，让底层知道引用计数变化了，不然底层无法维护计数！！
    //如果你不用缓存池，那么底层是可以正常计数的哈。
    public void AddRef()
    {
        if(mRes != null)
        {
            mRes.AddRef();
        }
    }
}
