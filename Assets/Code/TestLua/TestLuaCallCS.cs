using UnityEngine;
using XLua;

[LuaCallCSharp]
public class TestBase
{
    public static string content = "Hi Lua";

    //[LuaCallCSharp]
    public enum eDataType
    {
        eInt,
        eFloat = 10,
        eString,
    }

    public void ShowMyLove()
    {
        Debug.Log("this is my love");
    }
}

[LuaCallCSharp]
public class TestLuaCallCS : TestBase
{
    public void ShowSome(string s)
    {
        Debug.Log("this is content : " + s);
    }
}
