using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderMultiCompile : MonoBehaviour {

    public GameObject obj;

	// Use this for initialization
	void Start () {
        ResourceManager.Instance.mInitComplete = () =>
        {
            ResourceManager.Instance.LoadShader("allshader", "NewUnlitShader", (shader) =>
            {
                Material material = new Material(shader);
                obj.GetComponent<MeshRenderer>().sharedMaterial = material;
                material.EnableKeyword("ENABLE_COLOR");
            });
        };
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
