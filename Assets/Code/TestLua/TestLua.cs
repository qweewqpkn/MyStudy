using AssetLoad;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLua : MonoBehaviour {

    LuaState mLuaState;

	// Use this for initialization
	void Start () {
        mLuaState = new LuaState();
        mLuaState.Start();

        ResourceManager.Instance.mInitComplete = () =>
        {
            ResourceManager.Instance.LoadAB("cube", (ab) =>
            {
                Object[] obj = ab.LoadAllAssets();
                int i = 1;
            });
        };
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
