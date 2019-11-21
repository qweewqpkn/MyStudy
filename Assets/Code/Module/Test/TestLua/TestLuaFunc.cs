using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class TestLuaDelegate {

    public delegate void D1(GameObject obj, LuaTable table);
   
    public static void TestD1(int a, D1 d1)
    {
        GameObject obj = new GameObject();
        LuaTable t = LuaManager.Instance._LuaEnv.NewTable();
        d1(obj, t);
    }
}
