using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    public Button mButton;
    public RawImage mImage;
    public Image mSprite;
    public bool mStart;
    public int mFrame;

	// Use this for initialization
	void Start () {
        mButton.onClick.AddListener(() =>
        {
            ResourceManager.Instance.ReleaseAll();
            //ResourceManager.Instance.LoadSprite("sprite", "0001", (sprite) =>
            //{
            //    mSprite.sprite = sprite;
            //});
            //
            //ResourceManager.Instance.LoadSprite("sprite", "0001", (sprite) =>
            //{
            //    mSprite.sprite = sprite;
            //});
            //mStart = true;
            //mFrame = 1;
        });

        ResourceManager.Instance.LoadPrefab("lobby/test/cube", "cube", (obj) =>
        {
            if(obj != null)
            obj.name = "testTest";
        });
        ResourceManager.Instance.Release("lobby/test/cube", "cube");
        ResourceManager.Instance.LoadPrefab("lobby/test/cube", "cube", (obj)=>
        {
            if (obj != null)
                obj.name = "testTest1";
        });

        ResourceManager.Instance.LoadSprite("sprite", "0001", (sprite1) =>
        {
            mSprite.sprite = sprite1;
        });
        //ResourceManager.Instance.ReleaseAll();
        //ResourceManager.Instance.LoadPrefab("lobby/test/cube", "cube", (obj) =>
        //{
        //    if (obj != null)
        //        obj.name = "testTest2";
        //});
        //ResourceManager.Instance.ReleaseAll();
        //ResourceManager.Instance.LoadAB("lobby/test/cube", (ab) =>
        //{
        //});
    }
	
	// Update is called once per frame
	void Update () {
        //if(mStart)
        //{
        //    if(mFrame == 0)
        //    {
        //        //ResourceManager.Instance.ReleaseAll();
        //        ResourceManager.Instance.LoadPrefab("lobby/test/cube", "cube", (obj) =>
        //        {
        //            obj.name = "testTest2";
        //        });
        //    }
        //    mFrame--;
        //}
    }
}
