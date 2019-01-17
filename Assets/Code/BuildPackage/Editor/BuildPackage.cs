using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

//自动打包
public class BuildPackage : EditorWindow{

    private static Dictionary<string, string> argDic = new Dictionary<string, string>();
    private static string mOutputPath = "";
    private static string mPlatform = "PC";
    private static string mLogOpen = "";
    private static string mResPath = "";

    [MenuItem("Tools/打包版本")]
    static void OpenWindow()
    {
        BuildPackage window = EditorWindow.GetWindow<BuildPackage>();
        window.Show();
    }

    private void OnGUI()
    {
        //EditorGUILayout.TextField(new GUIContent(""))
    }

    static void BuildManual()
    {

    }

    //jenkins将会调用该函数
    static void BuildAuto()
    {
        InitArg();
        InitPlayerSetting(argDic["platform"]);
        CopyRes(mResPath);
        StartBuild(argDic["output"], argDic["platform"]);
    }

    static void CopyRes(string sourcePath)
    {
        FileUtility.CopyTo(sourcePath, Application.dataPath + "/StreamingAssets/ClientRes/Android");
    }

    static void StartBuild(string outputPath, string platform)
    {
        string[] level = GetBuildScene();
        string path = "";
        if (platform == "PC")
        {
            path = argDic["output"].Replace("\\", "/");
            path = string.Format("{0}/{1}_{2}.exe", path, "test", string.Format("{0:yyyy-MM-dd HH-mm-ss-fff}", DateTime.Now));
            BuildPipeline.BuildPlayer(level, path, BuildTarget.StandaloneWindows64, BuildOptions.None);
        }
        else if (platform == "ANDROID")
        {
            path = argDic["output"].Replace("\\", "/");
            path = string.Format("{0}/{1}_{2}.apk", path, "test", string.Format("{0:yyyy-MM-dd HH-mm-ss-fff}", DateTime.Now));
            BuildPipeline.BuildPlayer(level, path, BuildTarget.Android, BuildOptions.None);
        }
    }

    static void InitPlayerSetting(string platform)
    {
        PlayerSettings.productName = "xxx";
        PlayerSettings.applicationIdentifier = "com.test.game";

        //加上自定义的宏
        StringBuilder defines = new StringBuilder();
        if (argDic.ContainsKey("log"))
        {
            if (argDic["log"] == "true")
            {
                defines.Append("LOG_OPEN;");
            }
        }

        if (platform == "PC")
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines.ToString());
        }
        else if(platform == "ANDROID")
        {
            //签名
            //PlayerSettings.Android.keystoreName = Application.dataPath + "/Editor/AndroidKeyStore/game";
            //PlayerSettings.Android.keystorePass = "123456";
            //PlayerSettings.Android.keyaliasName = "game";
            //PlayerSettings.Android.keyaliasPass = "123456";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines.ToString());
        }
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
