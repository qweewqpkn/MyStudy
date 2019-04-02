//using AssetLoad;
//using LuaInterface;
//using System.Collections;
//using System.Collections.Generic;
using AssetLoad;
using UnityEngine;
using XLua;

public class TestLua : MonoBehaviour {

    LuaEnv mLuaEnv;

	// Use this for initialization
	void Start () {
        mLuaEnv = new LuaEnv();
        //mLuaEnv.AddLoader((ref string filepath) =>
        //{
        //    //TextAsset ta = ResourceManager.Instance.LoadLua();
        //    return ta.bytes;
        //});
        ResourceManager.Instance.LoadLua("luamain", "123");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
