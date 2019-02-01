using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class AutoBuildPC {

    protected static Dictionary<string, string> argDic = new Dictionary<string, string>();

    static void BuildPC()
    {
        InitArg();
        InitPlayerSetting();

        foreach(var item in argDic)
        {
            Debug.Log(item.Value);
        }
        string[] level = GetBuildScene();
        string path = argDic["output_path"].Replace("\\", "/") + "/" + string.Format("{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now);
        string name = "monk";
        path = string.Format("{0}/{1}.exe", path, name);
        BuildPipeline.BuildPlayer(level, path, BuildTarget.StandaloneWindows, BuildOptions.Development);
        //将你打包的资源拷贝到读取的路径下
        //FileUtility.CopyTo(PathManager.RES_PATH_WINDOWS, path + name + "_Data" + "/ClientRes/StandaloneWindows");
    }

    protected static string[] GetBuildScene()
    {
        List<string> sceneList = new List<string>();
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
            if (scene != null && scene.enabled)
            {
                sceneList.Add(scene.path);
            }
        }

        return sceneList.ToArray();
    }

    static void InitArg()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            //过滤出我们真正需要的参数,用*开头,IOS使用#作为标志,结果有特殊意义,导致传参失败
            if (args[i].StartsWith("*"))
            {
                string[] datas = args[i].Split('-');
                if (datas.Length == 2)
                {
                    argDic[datas[0].Substring(1)] = datas[1];
                }
            }
        }
    }

    static string GetArg(string key)
    {
        if(argDic.ContainsKey(key))
        {
            return argDic[key];
        }
        else
        {
            return "";
        }
    }

    static void InitPlayerSetting()
    {
        StringBuilder defines = new StringBuilder();
        //根据外部传入参数添加指定宏
        if (GetArg("log") == "true")
        {
            defines.Append("LOG_OPEN;");
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines.ToString());
    }
}
