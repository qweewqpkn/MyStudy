using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BuildAndroid {

    private static Dictionary<string, string> argDic = new Dictionary<string, string>();

    static void Build()
    {
        InitArg();
        InitPlayerSetting();
        string[] level = GetBuildScene();
        string path = "";
        if (argDic.ContainsKey("apk"))
        {
            path = argDic["apk"].Replace("\\", "/");
            path = string.Format("{0}/{1}_{2}.apk", path, "test", string.Format("{0:yyyy-MM-dd HH-mm-ss-fff}", DateTime.Now));
            BuildPipeline.BuildPlayer(level, path, BuildTarget.Android, BuildOptions.None);
        }
    }

    static void CopyTo()
    {
        string sourceDir = Application.dataPath + "/../../ClientRes/Android";
        string targetDir = Application.dataPath + "/StreamingAssets/ClientRes/Android";
        FileUtility.CopyTo(sourceDir, targetDir);
    }

    static void InitPlayerSetting()
    {
        PlayerSettings.productName = "xxx";
        StringBuilder defines = new StringBuilder();
        if (argDic.ContainsKey("log"))
        {
            if (argDic["log"] == "true")
            {
                defines.Append("LOG_OPEN;");
            }
        }

        if (argDic.ContainsKey("network"))
        {
            if (argDic["network"] == "true")
            {
                defines.Append("INTERNET;");
            }
        }
        defines.Append(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines.ToString());
    }

    static string[] GetBuildScene()
    {
        List<string> sceneList = new List<string>();
        for(int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
            if(scene != null && scene.enabled)
            {
                sceneList.Add(scene.path);
            }
        }

        return sceneList.ToArray();
    }

    static void InitArg()
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for(int i = 0; i < args.Length; i++)
        {
            if(args[i].StartsWith("#"))
            {
                string[] datas = args[i].Split('-');
                if(datas.Length == 2)
                {
                    argDic[datas[0].Substring(1)] = datas[1];
                }
            }
        }
    }
}
