using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XLua;

public class TestReflection : MonoBehaviour {
    enum TestState { eNone, eOne}

    private delegate void TestAction(int a, string b, LuaTable table, TestState state);
    private TestAction mMyAction;

    // Use this for initialization
    void Start () {
        Type typeInfo = typeof(TestAction);
        MethodInfo mi = typeInfo.GetMethod("Invoke");
        ParameterInfo[] pi = mi.GetParameters();
        for(int i = 0; i < pi.Length; i++)
        {
            Debug.Log(pi[i].ParameterType.IsValueType);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
