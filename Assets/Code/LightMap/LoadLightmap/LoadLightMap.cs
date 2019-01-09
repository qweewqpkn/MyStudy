using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLightMap : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        ResourceManager.Instance.mInitComplete = () =>
        {
            ResourceManager.Instance.LoadAsset<GameObject>("lightmaptest", (obj) =>
            {
                LightMapConfig config = obj.GetComponent<LightMapConfig>();
                config.SetUp();
            });
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
