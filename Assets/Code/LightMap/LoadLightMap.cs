using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLightMap : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        ResourceManager.Instance.LoadPrefabAsync("lightmaptest", "lightmaptest", (obj) =>
        {
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
