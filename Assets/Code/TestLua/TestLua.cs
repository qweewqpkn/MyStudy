using AssetLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class TestLua : MonoBehaviour {
    public Button mButton;

    public class DClass
    {
        public int f1;
        public int f2;
    }

    [CSharpCallLua]
    public delegate int ShowStr(int a, int b, out DClass dClass);

	// Use this for initialization
	void Start () {
        LuaManager.Instance.StartGame();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
