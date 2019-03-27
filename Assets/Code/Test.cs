﻿using AssetLoad;
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
            ResourceManager.Instance.ReleaseAll();
        });

        //ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1) =>
        //{
        //    if (Obj1 != null)
        //        Obj1.name = "test1";
        //});
        //ResourceManager.Instance.ReleaseAll();
        //GameObject Objt1 = ResourceManager.Instance.LoadPrefab("cube", "cube");
        //Objt1.name = "test5";
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test4";
        });
        ResourceManager.Instance.LoadPrefabAsync("cube", "cube", (Obj1, args) =>
        {
            if (Obj1 != null)
                Obj1.name = "test3";
        });
        //ResourceManager.Instance.LoadTextAsync("testconfig", "testconfig", (tx) =>
        //{
        //    if(tx == null)
        //    {
        //        return;
        //    }
        //
        //    byte[] byteCache = tx.bytes;
        //    MemoryStream ms = new MemoryStream(byteCache);
        //    string content = System.Text.Encoding.Default.GetString(byteCache);
        //});
        //ResourceManager.Instance.ReleaseAll();
        //ResourceManager.Instance.LoadTextAsync("testconfig", "testconfig", (tx) =>
        //{
        //    byte[] byteCache = tx.bytes;
        //    MemoryStream ms = new MemoryStream(byteCache);
        //    string content = System.Text.Encoding.Default.GetString(byteCache);
        //});
    }

    // Update is called once per frame
    void Update () {
        if(mStart)
        {
            if(mFrame == 0)
            {
                ResourceManager.Instance.ReleaseAll();
            }
            mFrame--;
        }
    }
}
