using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabAutoDestory : MonoBehaviour {

    public string mABName;
    public string mAssetName;

    void OnDestroy()
    {
        ResourceManager.Instance.Release(mABName, mAssetName, AssetType.ePrefab);
    }
}
