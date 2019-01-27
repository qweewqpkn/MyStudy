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

        AssetBundle ab = AssetBundle.LoadFromFile(ResourceManager.Instance.URL("texture", AssetType.eTexture));
        Texture obj1 = (Texture)ab.LoadAsset("texture");
        Texture obj2 = (Texture)ab.LoadAsset("texture");
        if(obj1 == obj2)
        {
            Debug.Log("fuck ==");
        }
        else
        {
            Debug.Log("fuck !=");
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
