using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabAutoDestory : MonoBehaviour {

    public string mABName;
    public string mAssetName;
    public GameObject mPrefab;

    void OnDestroy()
    {
        //注意：有一种情况，当我们手动调用ResourceManager.ReleaseAll后，我们再次重新加载了mABName和mAssetName对应prefab， 
        //然后我们销毁prefab的实例对象go，走到这里释放的时候，对应实例go的prefab资源已经在ReleaseAll销毁了，这里就不用再次销毁了，否则会销毁掉我们在ReleaseAll之后加载的prfab资源
        HRes res;
        string name = ResourceManager.GetResName(mABName, mAssetName, AssetType.ePrefab);
        if (ResourceManager.Instance.mResMap.TryGetValue(name, out res))
        {
            HPrefab prefab = res as HPrefab;
            if(prefab.GetPrefab() == mPrefab)
            {
                ResourceManager.Instance.Release(mABName, mAssetName, AssetType.ePrefab);
            }
        }
    }
}
