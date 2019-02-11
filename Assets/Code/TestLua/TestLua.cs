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
        mLuaState.DoFile("Main/Main.lua");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
