using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabAutoDestory : MonoBehaviour {

    public string mABName;

    void OnDestroy()
    {
        ResourceManager.Instance.Release(mABName, "");
    }
}
