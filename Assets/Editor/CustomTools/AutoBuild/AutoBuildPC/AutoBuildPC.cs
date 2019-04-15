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

    [MenuItem("Tools/Build/打包PC")]
    static void BuildManual()
    {
        Build(string.Format("*output_path-{0}", Path.GetFullPath(".") + "/Output/Windows"));
    }

    static void BuildAuto()
    {
        Build();
    }

    static void Build(string args = "")
    {
        InitArg(args);
        InitPlayerSetting();
        BuildRes.BuildAll();
        FileUtility.DeleteDirectory(PathManager.RES_STREAM_ROOT_PATH);
        FileUtility.CopyTo(PathManager.RES_LOCAL_ROOT_PATH + "/" + PathManager.GetEditorPlatform(), PathManager.RES_STREAM_ROOT_PATH + "/" + PathManager.GetEditorPlatform());
        string[] level = GetBuildScene();
        string outputPath = GetArg("output_path").Replace("\\", "/") + "/" + string.Format("{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now);
        string name = "SLG";
        string path = string.Format("{0}/{1}.exe", outputPath, name);
        BuildPipeline.BuildPlayer(level, path, BuildTarget.StandaloneWindows, BuildOptions.Development);
        FileUtility.ShowAndSelectFileInExplorer(outputPath);
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

    static void InitArg(string content)
    {
        string[] args;
        if (string.IsNullOrEmpty(content))
        {
            args = System.Environment.GetCommandLineArgs();
        }
        else
        {
            args = content.Split(' ');
        }

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
