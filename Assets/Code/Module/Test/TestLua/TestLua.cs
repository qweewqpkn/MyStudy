using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class TestLua : MonoBehaviour {
	// Use this for initialization
	void Start () {
        Debuger.Init();
        LuaManager.Instance.StartGame();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
