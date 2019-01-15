using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

//自动打包
public class BuildPackage {

    private static Dictionary<string, string> argDic = new Dictionary<string, string>();

    static void Build()
    {
        InitArg();
        InitPlayerSetting();
        AndroidSign();

        string[] level = GetBuildScene();
        string path = "";
        if (argDic.ContainsKey("output"))
        {
            if(argDic.ContainsKey("type"))
            {
                string type = argDic["type"];
                if(type == "PC")
                {
                    path = argDic["output"].Replace("\\", "/");
                    path = string.Format("{0}/{1}_{2}.exe", path, "test", string.Format("{0:yyyy-MM-dd HH-mm-ss-fff}", DateTime.Now));
                    BuildPipeline.BuildPlayer(level, path, BuildTarget.StandaloneWindows64, BuildOptions.None);
                }
                else if(type == "ANDROID")
                {
                    path = argDic["output"].Replace("\\", "/");
                    path = string.Format("{0}/{1}_{2}.apk", path, "test", string.Format("{0:yyyy-MM-dd HH-mm-ss-fff}", DateTime.Now));
                    BuildPipeline.BuildPlayer(level, path, BuildTarget.Android, BuildOptions.None);
                }
            }
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
        PlayerSettings.applicationIdentifier = "com.test.game";

        //加上所有的宏
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
        //defines.Append(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines.ToString());
    }

    [MenuItem("Tools/AndroidSign")]
    static void AndroidSign()
    {
        //PlayerSettings.Android.keystoreName = Application.dataPath + "/Editor/AndroidKeyStore/game";
        //PlayerSettings.Android.keystorePass = "123456";
        //PlayerSettings.Android.keyaliasName = "game";
        //PlayerSettings.Android.keyaliasPass = "123456";
    }

    //获取打包场景
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

    //获取外部传入参数
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
