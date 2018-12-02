﻿using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderMultiCompile : MonoBehaviour {

    public GameObject obj;

	// Use this for initialization
	void Start () {
        ResourceManager.Instance.LoadABAsset<AssetBundleManifest>("Assetbundle", "AssetBundleManifest", (manifest) =>
        {
            ResourceManager.Instance.LoadABAsset<Shader>("allshader", (shader) =>
            {
                Shader item = ResourceManager.Instance.GetShader("Unlit/NewUnlitShader");
                Material material = new Material(item);

                obj.GetComponent<MeshRenderer>().sharedMaterial = material;

                material.EnableKeyword("ENABLE_COLOR");
            });
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}