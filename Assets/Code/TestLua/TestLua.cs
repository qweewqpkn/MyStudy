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

        LuaTable mainTable = LuaManager.Instance._LuaEnv.Global.Get<LuaTable>("Main");
        ShowStr strFunc = mainTable.Get<ShowStr>("ShowStr");
        DClass dClass;
        int result = strFunc(11, 23, out dClass);
        Debug.Log("result : " + result);
        Debug.Log("dClass f1 : " + dClass.f1 + " " + "dClass f2 : " + dClass.f2);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
