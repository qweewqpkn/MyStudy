using System.Collections.Generic;
using AssetLoad;
using System.IO;
using UnityEngine;
using XLua;

public class LuaManager : SingletonMono<LuaManager> {

    LuaEnv mLuaEnv;
    LuaUpdater mLuaUpdater;
    private static string LUA_AB_NAME = "luamain";

    public LuaEnv _LuaEnv
    {
        get
        {
            return mLuaEnv;
        }
        private set
        {
            mLuaEnv = value;
        }
    }

    protected override void Init()
    {
        mLuaEnv = new LuaEnv();
        InitLoader();
    }

    void InitLoader()
    {
        mLuaEnv.AddLoader((ref string fileName) =>
        {
            string name = fileName.ToLower();

#if UNITY_EDITOR
            string filePath = string.Format("{0}/{1}", PathManager.LUA_ROOT_PATH, name.Replace(".", "/"));
            if (!filePath.Contains(".lua"))
            {
                filePath = filePath + ".lua";
            }
            byte[] bytes = File.ReadAllBytes(filePath);
            return bytes;
#else
            string filePath = name.Replace(".", "_");
            if (!filePath.Contains(".lua"))
            {
                filePath = filePath + ".lua";
            }
            TextAsset ta = ResourceManager.Instance.LoadLua(LUA_AB_NAME, filePath);
            return ta.bytes;
#endif
        });
    }

    //重新加载lua文件(热重载)
    public void ReLoadLua(string name)
    {
        if(mLuaEnv != null)
        {
            DoString(string.Format("package.loaded['{0}'] = nil", name));
            LoadLua(name);
        }
    }

    //加载某个lua文件
    public void LoadLua(string name)
    {
        if(mLuaEnv != null)
        {
            DoString(string.Format("require '{0}'", name));
        }
    }

    //执行语句
    public void DoString(string str)
    {
        if(mLuaEnv != null)
        {
            mLuaEnv.DoString(str);
        }
    }

    public void StartGame()
    {
        LoadLua("Main");
        DoString("Main.Start()");
        InitUpdate();
    }

    void InitUpdate()
    {
        mLuaUpdater = gameObject.AddComponent<LuaUpdater>();
        mLuaUpdater.OnInit(mLuaEnv);
    }
	
	// Update is called once per frame
	void Update () {
		if(mLuaEnv != null)
        {
            mLuaEnv.Tick();
        }
	}

    void Dispose()
    {
        if(mLuaEnv != null)
        {
            mLuaEnv.Dispose();
        }
    }
}
