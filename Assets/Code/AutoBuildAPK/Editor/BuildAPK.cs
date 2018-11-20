using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BuildAPK {

    private static Dictionary<string, string> argDic = new Dictionary<string, string>();

    static void Build()
    {
        //InitArg();
        //InitPlayerSetting();
        string[] level = GetBuildScene();
        string apkPath = Application.dataPath + "/../APK/1.apk"; 
        BuildPipeline.BuildPlayer(level, apkPath, BuildTarget.Android, BuildOptions.None);
    }
    static void InitPlayerSetting()
    {
        PlayerSettings.productName = "斗地主";
        StringBuilder defines = new StringBuilder();
        if (argDic["log"] == "true")
        {
            defines.Append("LOG_OPEN;");
        }
        if (argDic["network"] == "true")
        {
            defines.Append("INTERNET;");
        }
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

        //FileStream fs = File.Create(Application.dataPath + "/ArgLog.txt");
        //StreamWriter sw = new StreamWriter(fs);
        //sw.WriteLine(args.Length);
        //for(int i = 0; i < args.Length; i++)
        //{
        //    sw.WriteLine(args[i]);
        //}
        //sw.Close();
        //fs.Close();
    }
}
