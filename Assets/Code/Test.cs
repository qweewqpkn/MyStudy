using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    public Button mButton;
    public RawImage mImage;

	// Use this for initialization
	void Start () {
        ResourceManager.Instance.mInitComplete = () =>
        {

            mButton.onClick.AddListener(() =>
            {
                ResourceManager.Instance.LoadTexture("texture", "texture", (tex) =>
                {
                    mImage.texture = tex;
                });

                ResourceManager.Instance.LoadTexture("texture", "texture", (tex) =>
                {
                    mImage.texture = tex;
                });

                ResourceManager.Instance.LoadTexture("texture", "texture", (tex) =>
                {
                    mImage.texture = tex;
                });

                ResourceManager.Instance.ReleaseAll();
            });

            ResourceManager.Instance.LoadPrefab("cube", "cube.bytess", (obj) =>
            {
                obj.name = "testTest";
            });

            ResourceManager.Instance.LoadPrefab("cube", "cube.bytess", (obj) =>
            {
                obj.name = "testTest";
            });

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
