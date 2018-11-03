using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabAutoDestory : MonoBehaviour {

    public string mAssetBundleName;

    void OnDestroy()
    {
        ResourceManager.Instance.ReleaseAB(mAssetBundleName);
    }
}
