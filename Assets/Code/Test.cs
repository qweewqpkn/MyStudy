using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ResourceManager.Instance.mInitComplete = () =>
        {
            ResourceManager.Instance.LoadPrefab("cube", "cube.bytess", (obj) =>
            {
                obj.name = "testTest";
            });
        };
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
