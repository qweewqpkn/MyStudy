using AssetLoad;
using BinaryConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TestResLoad : MonoBehaviour {

    public Button mButton;
    public RawImage mImage;
    public Image mSprite;
    public Image mSprite1;
    public AudioSource mAS;
    public GameObject mObj;
    public TextMeshProUGUI mText;
    public bool mStart;
    public int mFrame;
    private int mFontState;

	// Use this for initialization
	void Start () {
        Debuger.Init();
        Debuger.SwitchModule("test", true);
        mButton.onClick.AddListener(() =>
        {
            //LoadTest1();
            //LoadTest2();
            //LoadTest3();
            //LoadTest4();
            //LoadTest5();
            //LoadTest6();
            //LoadTest7();
            //LoadTest8();
            //LoadTest9();
            //LoadSprite();
            //PreLoad();
            //ReleaseTest();
            LoadFont();
        });
    }

    void LoadFont()
    {
        if(mFontState == 0)
        {
            mFontState = 1;
            ResourceManager.Instance.LoadFontAsync("Font", "Test Anton SDF", (fontAsset) =>
            {
                mText.font = fontAsset;
            });
        }
        else
        {
            mFontState = 0;
            ResourceManager.Instance.LoadFontAsync("Font", "Test Bangers SDF", (fontAsset) =>
            {
                mText.font = fontAsset;
            });
        }
    }

    void ReleaseTest()
    {
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test1";
        });

        //ResourceManager.Instance.Release("cube");
    }

    //使用预加载的方式
    void PreLoad()
    {
        StartCoroutine(CoPreLoad());
    }

    IEnumerator CoPreLoad()
    {
        Debuger.Log("test", "111 Frame Count : " + Time.frameCount);
        yield return ResourceManager.Instance.PreLoadPrefabCoRequest("cube", "cube");
        Debuger.Log("test", "start Frame Count : " + Time.frameCount);
        //ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (obj, args)=>
        //{
        //    Debuger.Log("test", "over Frame Count : " + Time.frameCount);
        //});

        GameObject obj1 = ResourceManager.Instance.LoadPrefab("cube", "cube");
    }

    IEnumerator CoLoad()
    {
        Debug.Log("start frame count : " + Time.frameCount);
        AsyncRequest request = ResourceManager.Instance.PreLoadPrefabCoRequest("cube", "cube");
        yield return request;
        Debug.Log("over frame count : " + Time.frameCount);
        GameObject obj = request.Asset as GameObject;
        obj.name = "success ok";

        Debug.Log("start frame count : " + Time.frameCount);
        AsyncRequest request1 = ResourceManager.Instance.PreLoadPrefabCoRequest("cube", "cube");
        yield return request1;
        Debug.Log("over frame count : " + Time.frameCount);
        GameObject obj1 = request.Asset as GameObject;
        obj1.name = "success ok1";
    }

    void LoadConfig()
    {
        BinaryConfigManager.Instance.LoadAllBinaryData("config", ()=>
        {
            //加载完成
            BinaryConfigManager.Instance.LoadBinaryData<TestConfig>("TestConfig");
            BinaryConfigManager.Instance.LoadBinaryData<TestOtherConfig>("TestOtherConfig");
            BinaryConfigManager.Instance.LoadBinaryData<AchieveConfig>("AchieveConfig");
        });
    }

    void LoadTest1()
    {
        //测试1
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test1";
        });
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test2";
        });
        ResourceManager.Instance.ReleaseAll();
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test3";
        });
    }

    void LoadTest2()
    {
        //测试2
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test1";
        });
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test2";
        });
    }

    void LoadTest3()
    {
        //测试3
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test1";
        });
        ResourceManager.Instance.ReleaseAll();
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test2";
        });
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test3";
        });
    }

    void LoadTest4()
    {
        //测试4
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test1";
        });
        GameObject objN = ResourceManager.Instance.LoadPrefab("cube", "cube");
        objN.name = "test2x";
    }

    void LoadTest5()
    {
        //测试5
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test1";
        });
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test2";
        });
        GameObject objN = ResourceManager.Instance.LoadPrefab("cube", "cube");
        objN.name = "test2x";
    }

    void LoadTest6()
    {
        //测试6
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test1";
        });
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test2";
        });
        GameObject objN2 = ResourceManager.Instance.LoadPrefab("cube", "cube");
        objN2.name = "test2x";
        ResourceManager.Instance.ReleaseAll();
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test3";
        });
        GameObject objN3 = ResourceManager.Instance.LoadPrefab("cube", "cube");
        objN3.name = "test3x";
    }

    void LoadTest7()
    {
        //测试7
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test1";
        });
        GameObject objN4 = ResourceManager.Instance.LoadPrefab("cube", "cube");
        objN4.name = "test2x";
        ResourceManager.Instance.ReleaseAll();
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test3";
        });
        GameObject objN5 = ResourceManager.Instance.LoadPrefab("cube", "cube");
        objN5.name = "test3x";
    }

    void LoadTest8()
    {
        //测试8
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test1";
        });
        ResourceManager.Instance.ReleaseAll();
        GameObject objN6 = ResourceManager.Instance.LoadPrefab("cube", "cube");
        objN6.name = "test3x";
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test3";
        });
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test4";
        });
    }

    void LoadTest9()
    {
        ResourceManager.Instance.LoadAB("cube");
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (res, args) =>
        {
            if (res != null)
                res.name = "test2";
        });
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (res, args) =>
        {
            if (res != null)
                res.name = "test3";
        });
        ResourceManager.Instance.ReleaseAll();
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (res, args) =>
        {
            res.name = "test1";
        });
    }

    void LoadSprite()
    {
        ResourceManager.Instance.LoadSpriteAsync("sprite", "0001", (sprite)=>
        {
            mSprite.sprite = sprite;
        });

        ResourceManager.Instance.LoadSpriteAsync("sprite", "0001", (sprite) =>
        {
            mSprite1.sprite = sprite;
        });

        ResourceManager.Instance.ReleaseAll();

        ResourceManager.Instance.LoadSpriteAsync("sprite", "0003", (sprite) =>
        {
            mSprite1.sprite = sprite;
        });
    }

    // Update is called once per frame
    void Update () {
        if(mStart)
        {
            if(mFrame == 0)
            {
                //ResourceManager.Instance.ReleaseAll();
            }
            mFrame--;
        }
    }
}
