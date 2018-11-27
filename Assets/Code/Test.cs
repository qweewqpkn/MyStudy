using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ResourceManager.Instance.LoadABAsset<AssetBundleManifest>("Assetbundle", "AssetBundleManifest", (asset) =>
        {
            ResourceManager.Instance.LoadABAsset<GameObject>("material", (asset1) =>
            {
                GameObject obj = asset1;
            });

            ResourceManager.Instance.LoadABAsset<GameObject>("cube", (asset1) =>
            {
                GameObject obj = asset1;
            });

            ResourceManager.Instance.LoadAsset<byte[]>("mytext.txt", (asset3) => {
                byte[] bytes = asset3;
            });
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
