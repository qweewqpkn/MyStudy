using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLightMap : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //ResourceManager.Instance.LoadManifest(() =>
        //{
            //ResourceManager.Instance.LoadAllShader("allshader", (name) =>
            //{
            //    ResourceManager.Instance.LoadPrefabGO("lightmaptest", "lightmaptest", (name1, obj) =>
            //    {
            //        LightMapConfig config = obj.GetComponent<LightMapConfig>();
            //        config.SetUp();
            //
            //        Shader shader = ResourceManager.Instance.GetShader("LH/LightMap");
            //        Renderer renderer = obj.transform.Find("Plane").GetComponent<Renderer>();
            //        renderer.material.shader = shader;
            //    });
            //});
        //});
    }

    // Update is called once per frame
    void Update()
    {

    }
}
