using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BuildPC
{

    private static Dictionary<string, string> argDic = new Dictionary<string, string>();

    static void Build()
    {
        InitArg();
        InitPlayerSetting();
        string[] level = GetBuildScene();

        string path = "";
        if(argDic.ContainsKey("apk"))
        {
            path = argDic["apk"].Replace("\\", "/");
            path = string.Format("{0}/{1}_{2}.exe", path, "test", string.Format("{0:yyyy-MM-dd HH-mm-ss-fff}", DateTime.Now));
            BuildPipeline.BuildPlayer(level, path, BuildTarget.StandaloneWindows64, BuildOptions.None);
        }
    }

    static void CopyTo()
    {
        string sourceDir = Application.dataPath + "/../../ClientRes/Android";
        string targetDir = Application.dataPath + "/StreamingAssets/ClientRes/Android";
        CopyTo(sourceDir, targetDir);
    }

    static void CopyTo(string sourceDir, string targetDir)
    {
        if (!Directory.Exists(sourceDir))
        {
            Debug.LogError("sourcePath is not exist");
            return;
        }

        if (Directory.Exists(targetDir))
        {
            Directory.Delete(targetDir, true);
        }
        Directory.CreateDirectory(targetDir);

        string[] files = Directory.GetFiles(sourceDir);
        if (files != null)
        {
            for (int i = 0; i < files.Length; i++)
            {
                File.Copy(files[i], targetDir + "/" + Path.GetFileName(files[i]));
            }
        }

        string[] curDirs = Directory.GetDirectories(sourceDir);
        if (curDirs != null)
        {
            for (int i = 0; i < curDirs.Length; i++)
            {
                string path = curDirs[i].Substring(curDirs[i].LastIndexOf("\\") + 1);
                CopyTo(curDirs[i], targetDir + "/" + path);
            }
        }
    }

    static void InitPlayerSetting()
    {
        PlayerSettings.productName = "xxx";
        StringBuilder defines = new StringBuilder();
        if(argDic.ContainsKey("log"))
        {
            if (argDic["log"] == "true")
            {
                defines.Append("LOG_OPEN;");
            }
        }

        if(argDic.ContainsKey("network"))
        {
            if (argDic["network"] == "true")
            {
                defines.Append("INTERNET;");
            }
        }
        defines.Append(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone));
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines.ToString());
    }

    static string[] GetBuildScene()
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
            if (args[i].StartsWith("#"))
            {
                string[] datas = args[i].Split('-');
                if (datas.Length == 2)
                {
                    argDic[datas[0].Substring(1)] = datas[1];
                }
            }
        }
    }
}
