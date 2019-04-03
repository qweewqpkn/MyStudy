using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLua : MonoBehaviour {
    public Button mButton;

	// Use this for initialization
	void Start () {
        LuaManager.Instance.StartGame();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
