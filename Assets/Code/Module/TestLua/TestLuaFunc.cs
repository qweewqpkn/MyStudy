using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class TestLuaFunc : MonoBehaviour {

    public delegate void TestFunc(string s);
    private TestFunc mFunc;

    public void Init(TestFunc func)
    {
        mFunc = func;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(mFunc != null)
        {
            mFunc("sdfasfa111111111111xx");
        }
	}
}
