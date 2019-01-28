using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AutoBuildAndroid {

    protected static Dictionary<string, string> argDic = new Dictionary<string, string>();

    static void BuildAndroid()
    {
        InitArg();
        InitPlayerSetting();
        //将你打包的资源拷贝到StreamingAssets中
        //FileUtility.CopyTo(Application.dataPath + "/../../ClientRes/Android", Application.dataPath + "/StreamingAssets/ClientRes/Android");
        string[] level = GetBuildScene();
        string path = argDic["output_path"].Replace("\\", "/");
        path = string.Format("{0}/{1}_{2}.apk", path, "doudizhu", string.Format("{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now));
        BuildPipeline.BuildPlayer(level, path, BuildTarget.Android, BuildOptions.None);
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
        PlayerSettings.productName = "名字";
        PlayerSettings.companyName = "公司名";
        PlayerSettings.applicationIdentifier = "com.monk.game";
        //设置签名
        PlayerSettings.Android.keystoreName = Application.dataPath + "/Editor/AndroidKeyStore/game";
        PlayerSettings.Android.keystorePass = "123";
        PlayerSettings.Android.keyaliasName = "game";
        PlayerSettings.Android.keyaliasPass = "123";

        //设置icon
        string iconName = "icon_512x512";
        Texture2D texture = AssetDatabase.LoadAssetAtPath(string.Format("Assets/Resources/{0}.png", iconName), typeof(Texture2D)) as Texture2D;
        int[] iconSize = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Android);
        Texture2D[] textureArray = new Texture2D[iconSize.Length];
        for (int i = 0; i < textureArray.Length; i++)
        {
            textureArray[i] = texture;
        }
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, textureArray);
        AssetDatabase.SaveAssets();

        StringBuilder defines = new StringBuilder();
        //根据外部传入参数添加指定宏
        if (argDic["log"] == "true")
        {
            defines.Append("LOG_OPEN;");
        }
        if (argDic["log_file"] == "true")
        {
            defines.Append("LOG_TO_FILE;");
        }
        if (argDic["network"] == "true")
        {
            defines.Append("INTERNET;");
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defines.ToString());
    }

    //获取通过shell脚本传入的参数
    static void InitArg()
    {
        string[] args = System.Environment.GetCommandLineArgs();
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
