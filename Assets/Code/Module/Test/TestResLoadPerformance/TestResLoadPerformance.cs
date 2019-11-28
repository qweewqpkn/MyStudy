using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestResLoadPerformance : MonoBehaviour {

    public Text mAllTimeTxt;
    public Text mOneByOneTimeTxt;

    public Button mBtnLoadAll;
    public Button mBtnLoadOneByOne;

	// Use this for initialization
	void Start () {
        mBtnLoadAll.onClick.AddListener(() =>
        {
            LoadAll();
        });

        mBtnLoadOneByOne.onClick.AddListener(() =>
        {
            LoadOneByOne();
        });
    }

    void LoadAll()
    {
        float startTime = Time.realtimeSinceStartup;
        for(int i = 1; i <= 15; i++)
        {
            ResourceManager.Instance.LoadTexture("alltexture", "texture" + i);
        }
        float costTime = Time.realtimeSinceStartup - startTime;
        mAllTimeTxt.text = "All" + costTime.ToString();
    }

    void LoadOneByOne()
    {
        float startTime = Time.realtimeSinceStartup;
        for (int i = 1; i <= 15; i++)
        {
            ResourceManager.Instance.LoadTexture("texture" + i, "texture" + i);
        }
        float costTime = Time.realtimeSinceStartup - startTime;
        mOneByOneTimeTxt.text = "OneByOne" + costTime.ToString();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
