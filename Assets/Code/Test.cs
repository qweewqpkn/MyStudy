using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    public Button mButton;
    public RawImage mImage;
    public Image mSprite;

	// Use this for initialization
	void Start () {
        ResourceManager.Instance.mInitComplete = () =>
        {

            mButton.onClick.AddListener(() =>
            {
                ResourceManager.Instance.LoadSprite("sprite", "0001", (sprite) =>
                {
                    mSprite.sprite = sprite;
                });

                ResourceManager.Instance.LoadSprite("sprite", "0001", (sprite) =>
                {
                    mSprite.sprite = sprite;
                });

                //ResourceManager.Instance.ReleaseAll();
            });

            ResourceManager.Instance.LoadPrefab("lobby/test/cube", "cube", (obj) =>
            {
                obj.name = "testTest";
            });

            TextAsset lua = ResourceManager.Instance.LoadLua("lobby", "Main.lua");
            TextAsset lua1 = ResourceManager.Instance.LoadLua("main", "Test.lua");
        };
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
