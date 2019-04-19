using AssetLoad;
using BinaryConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class Test : MonoBehaviour {

    public Button mButton;
    public RawImage mImage;
    public Image mSprite;
    public Image mSprite1;
    public AudioSource mAS;
    public GameObject mObj;
    public bool mStart;
    public int mFrame;

	// Use this for initialization
	void Start () {
        mButton.onClick.AddListener(() =>
        {
            for (int i = 0; i < 10000; i++)
            {
                Debuger.Log("other", "xixixi");
            }
            //ResourceManager.Instance.ReleaseAll();
        });

        Debuger.Init();
        Debuger.SwitchModule("other", true);
        //CoroutineUtility.Instance.StartCoroutine(CoLoad());
    }

    IEnumerator CoLoad()
    {
        Debug.Log("start frame count : " + Time.frameCount);
        AsyncRequest request = ResourceManager.Instance.LoadPrefabAsync("cube", "cube");
        yield return request;
        Debug.Log("over frame count : " + Time.frameCount);
        GameObject obj = request.Asset as GameObject;
        obj.name = "success ok";

        Debug.Log("start frame count : " + Time.frameCount);
        AsyncRequest request1 = ResourceManager.Instance.LoadPrefabAsync("cube", "cube");
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
