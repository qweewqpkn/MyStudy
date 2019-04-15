using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AutoBuildAndroid {

    protected static Dictionary<string, string> argDic = new Dictionary<string, string>();

    [MenuItem("Tools/Build/打包Android")]
    static void BuildManual()
    {
        Build(string.Format("*output_path-{0}", Path.GetFullPath(".") + "/Output/Android"));
    }

    static void BuildAuto()
    {
        Build();
    }
    
    static void Build(string args = "")
    {
        if(PathManager.GetEditorPlatform() != "Android")
        {
            Debug.LogError("当前不是Android平台,请切换后再打包!");
        }
        else
        {
            InitArg(args);
            InitPlayerSetting();
            BuildRes.BuildAll();
            if (Directory.Exists(PathManager.RES_STREAM_ROOT_PATH))
            {
                Directory.Delete(PathManager.RES_STREAM_ROOT_PATH, true);
            }
            BuildRes.CopyResToStreamingAssets(PathManager.RES_LOCAL_ROOT_PATH + "/" + PathManager.GetEditorPlatform(), PathManager.RES_STREAM_ROOT_PATH + "/" + PathManager.GetEditorPlatform());
            string[] level = GetBuildScene();
            string path = GetArg("output_path").Replace("\\", "/");
            FileUtility.CreateDirectory(path);
            path = string.Format("{0}/{1}_{2}.apk", path, "SLG", string.Format("{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now));
            //使用5.6的unity path是不能含有Assets路径的，但是2017就可以使用包含有Assets的路径
            BuildPipeline.BuildPlayer(level, path, BuildTarget.Android, BuildOptions.None);
            Debug.Log("打包APK完成!");
        }
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

    static void InitPlayerSetting()
    {
        PlayerSettings.productName = "monk";
        PlayerSettings.companyName = "monk";
        PlayerSettings.applicationIdentifier = "com.monk.game";
        //设置签名
        //layerSettings.Android.keystoreName = Application.dataPath + "/Editor/AndroidKeyStore/game";
        //layerSettings.Android.keystorePass = "123";
        //layerSettings.Android.keyaliasName = "game";
        //layerSettings.Android.keyaliasPass = "123";

        //设置icon
        //string iconName = "icon_512x512";
        //Texture2D texture = AssetDatabase.LoadAssetAtPath(string.Format("Assets/Resources/{0}.png", iconName), typeof(Texture2D)) as Texture2D;
        //int[] iconSize = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Android);
        //Texture2D[] textureArray = new Texture2D[iconSize.Length];
        //for (int i = 0; i < textureArray.Length; i++)
        //{
        //    textureArray[i] = texture;
        //}
        //PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, textureArray);
        //AssetDatabase.SaveAssets();

        StringBuilder defines = new StringBuilder();
        //根据外部传入参数添加指定宏
        if (GetArg("log") == "true")
        {
            defines.Append("LOG_OPEN;");
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines.ToString());
    }

    static string GetArg(string key)
    {
        if (argDic.ContainsKey(key))
        {
            return argDic[key];
        }
        else
        {
            return "";
        }
    }

    //获取通过shell脚本传入的参数
    static void InitArg(string content = "")
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
}
