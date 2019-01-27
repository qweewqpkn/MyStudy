using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode.Custom;
#endif
using UnityEngine;

public class AutoBuild {

    private static Dictionary<string, string> argDic = new Dictionary<string, string>();

    static void BuildAPK()
    {
        InitArg();
        InitPlayerSetting(BuildTargetGroup.Android);
        SetAndroidKeyStore();
        SetDefaultIcon();
        DeleteAndroidRes();
        CopyLocalVersion();
        CopyAndroidRes();
        BuildResource();
        CopyAssets.CopyAndroidAssets();
        MergeConfigToBin.MergeConfigAndroid();
        CopyTo(Application.dataPath + "/../../ClientRes/Android", Application.dataPath + "/StreamingAssets/ClientRes/Android");

        string[] level = GetBuildScene();
        string apkPath = argDic["output_path"].Replace("\\", "/");
        //DeletePathChild(apkPath);

        if (argDic.ContainsKey("anysdk") && argDic["anysdk"] == "true")
        {
            apkPath += "/sdk/base_sdk.apk";
        }
        else
        {
            apkPath = string.Format("{0}/{1}_{2}.apk", apkPath, "doudizhu", string.Format("{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now));
        }
        BuildPipeline.BuildPlayer(level, apkPath, BuildTarget.Android, BuildOptions.None);
    }

    static void BuildPC()
    {
        InitArg();
        InitPlayerSetting(BuildTargetGroup.Standalone);
        DeletePCRes();
        BuildResource();
        CopyAssets.CopyStandaloneWindowsAssets();
        MergeConfigToBin.MergeConfigPC();

        string[] level = GetBuildScene();
        string exePath = argDic["output_path"].Replace("\\", "/");
        //DeletePathChild(exePath);
        string sub_file_name = string.Format("{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now);
        argDic["sub_file_name"] = sub_file_name;
        argDic["exe_name"] = "doudizhu";
        exePath = string.Format("{0}/{1}/{2}.exe", exePath, sub_file_name, "doudizhu");
        BuildPipeline.BuildPlayer(level, exePath, BuildTarget.StandaloneWindows, BuildOptions.Development);

        CopyPcRes();
    }

    static void BuildIOS()
    {
        InitArg();
        InitPlayerSetting(BuildTargetGroup.iOS);
        BuildResource();
        CopyAssets.CopyIOSAssets();
        MergeConfigToBin.MergeConfigIOS();
        CopyTo(Application.dataPath + "/../../ClientRes/IOS", Application.dataPath + "/StreamingAssets/ClientRes/IOS");

        string[] level = GetBuildScene();
        string outputPath = argDic["output_path"].Replace("\\", "/");
        if(Directory.Exists(outputPath))
        {
            Directory.Delete(outputPath, true);
        }
        BuildPipeline.BuildPlayer(level, outputPath, BuildTarget.iOS, BuildOptions.None);
    }

#if UNITY_IOS
    [PostProcessBuildAttribute(2)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.iOS)
        {
            ModifyPBXProject(path);
            ModifyPList(path);
        }
    }

    //修改PBXProject
    static void ModifyPBXProject(string path)
    {
        // Read.
        string projectPath = PBXProject.GetPBXProjectPath(path);
        PBXProject project = new PBXProject();
        project.ReadFromString(File.ReadAllText(projectPath));
        string targetName = PBXProject.GetUnityTargetName();
        string targetGUID = project.TargetGuidByName(targetName);

        // Frameworks 
        project.AddFrameworkToProject(targetGUID, "WebKit.framework", false);
        project.AddFrameworkToProject(targetGUID, "MobileCoreServices.framework", false);
        project.AddFrameworkToProject(targetGUID, "CoreFoundation.framework", false);

        //属性
        project.SetBuildProperty(targetGUID, "ENABLE_BITCODE", "NO");
        project.SetTargetAttributes("ProvisioningStyle", "Manual");

        //设置开发版本的证书
        project.SetBuildProperty(targetGUID, "PROVISIONING_PROFILE", "56a4a604-de21-4f53-a866-6a2f8477d15c");
        project.SetBuildProperty(targetGUID, "CODE_SIGN_IDENTITY", "iPhone Developer");
        project.SetBuildProperty(targetGUID, "CODE_SIGN_IDENTITY[sdk=iphoneos*]", "iPhone Developer: zhang min (AC6ZKF5S5L)");

        File.WriteAllText(projectPath, project.WriteToString());
    }

    //修改plist
    static void ModifyPList(string path)
    {
        string plistPath = Path.Combine(path, "Info.plist");
        PlistDocument pList = new PlistDocument();
        pList.ReadFromFile(plistPath);
        PlistElementDict rootDict = pList.root;

        rootDict.SetString("NSCameraUsageDescription", "我们需要使用摄像头权限");
        rootDict.SetString("NSLocationWhenInUseUsageDescription", "我们需要使用定位权限");

        pList.WriteToFile(plistPath);
    }
#endif

    static void BuildResource()
    {
        List<UnityEngine.Object> list = new List<UnityEngine.Object>();
        string[] files = Directory.GetFiles(Application.dataPath + "/Export", "*", SearchOption.AllDirectories);
        for(int i = 0; i < files.Length; i++)
        {
            if(!files[i].EndsWith("meta"))
            {
                string path = files[i].Substring(files[i].IndexOf("Assets/"));
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                list.Add(obj);
            }
        }

        ResouceExporter.Execute(list.ToArray());
    }

    static void CopyTo(string sourceDir, string targetDir)
    {
        if(!Directory.Exists(sourceDir))
        {
            Debug.LogError("sourcePath is not exist");
            return;
        }

        if(Directory.Exists(targetDir))
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
        if(curDirs != null)
        {
            for(int i = 0; i < curDirs.Length; i++)
            {
                //为了支持ios
                curDirs[i] = curDirs[i].Replace("\\", "/");
                string path = curDirs[i].Substring(curDirs[i].LastIndexOf("/") + 1);
                CopyTo(curDirs[i], targetDir + "/" + path);
            }
        }
    }

    static void InitPlayerSetting(BuildTargetGroup btg)
    {
        PlayerSettings.productName = "久久斗地主";
        PlayerSettings.companyName = "Bltech";
        PlayerSettings.applicationIdentifier = "com.jiuyou.game99";
        if(btg == BuildTargetGroup.iOS)
        {
            PlayerSettings.applicationIdentifier = "com.jiuyou99.game99";
        }

        StringBuilder defines = new StringBuilder();
        //添加默认宏
        defines.Append("MULTI_STATE;");
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
        if (argDic.ContainsKey("anysdk"))
        {
            if (argDic["anysdk"] == "true")
            {
                defines.Append("ANYSDK;");
            }
        }
        if (argDic.ContainsKey("hwsdk"))
        {
            if (argDic["hwsdk"] == "true")
            {
                defines.Append("HWSDK;");
            }

            PlayerSettings.applicationIdentifier = "com.jiuyou.doudizhu99.huawei";
        }
        if (argDic.ContainsKey("umeng_sdk"))
        {
            if (argDic["umeng_sdk"] == "true")
            {
                defines.Append("UMENG_SDK;");
            }
        }
        if (argDic.ContainsKey("open_install_sdk"))
        {
            if (argDic["open_install_sdk"] == "true")
            {
                defines.Append("OPEN_INSTALL_SDK;");
            }
        }

        //修复重复宏越来越多的BUG
        //defines.Append(PlayerSettings.GetScriptingDefineSymbolsForGroup(btg));
        PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, defines.ToString());
    }

    /// <summary>
    /// 设置安卓包签名
    /// </summary>
    [MenuItem("Tools/AndroidSign")]
    static void SetAndroidKeyStore()
    {
        //if (Application.platform == RuntimePlatform.Android)
        {
            PlayerSettings.Android.keystoreName = Application.dataPath + "/Editor/AndroidKeyStore/game99_1";
            PlayerSettings.Android.keystorePass = "jiuyou2017";
            PlayerSettings.Android.keyaliasName = "game99";
            PlayerSettings.Android.keyaliasPass = "jiuyou2017";
        }
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
            if(args[i].StartsWith("*"))
            {
                string[] datas = args[i].Split('-');
                if(datas.Length == 2)
                {
                    argDic[datas[0].Substring(1)] = datas[1];
                }
            }
        }
    }

    static void CopyPcRes()
    {
        string sourceDir = Application.dataPath + "/../../ClientRes/StandaloneWindows";
        string targetDir = argDic["output_path"] + "/" + argDic["sub_file_name"] + "/" + argDic["exe_name"] + "_Data" + "/ClientRes/StandaloneWindows";
        CopyTo(sourceDir, targetDir);
    }

    static void DeletePathChild(string path)
    {
        if(!Directory.Exists(path))
        {
            return;
        }

        string[] files = Directory.GetFiles(path);
        if (files != null)
        {
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo fi = new FileInfo(files[i]);
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;
                File.Delete(files[i]);
            }
        }

        if (Directory.Exists(path))
        {
            string[] curDirs = Directory.GetDirectories(path);
            if (curDirs != null)
            {
                for (int i = 0; i < curDirs.Length; i++)
                {
                    Directory.Delete(curDirs[i], true);
                }
            }
        }
    }

    static void DeleteAndroidRes()
    {
        string assetbundle_path = Application.dataPath + "/../../ClientRes/Android/assetbundle";
        DeletePathChild(assetbundle_path);
        string xml_path = Application.dataPath + "/../../ClientRes/Android/xml";
        DeletePathChild(xml_path);
    }

    static void DeletePCRes()
    {
        string assetbundle_path = Application.dataPath + "/../../ClientRes/StandaloneWindows/assetbundle";
        DeletePathChild(assetbundle_path);
        string xml_path = Application.dataPath + "/../../ClientRes/StandaloneWindows/xml";
        DeletePathChild(xml_path);
    }

    /// <summary>
    /// 拷贝版本文件
    /// </summary>
    static void CopyLocalVersion()
    {
        string target_path = Application.dataPath + "/../../ClientRes/Android/xml/";
        string source_path = Application.dataPath + "/../../ClientRes/localVersion.xml";

        File.Copy(source_path, target_path + "/" + "localVersion.xml");
    }

    /// <summary>
    /// 设置安卓应用图标
    /// </summary>
    static void SetDefaultIcon()
    {
        string iconName = "icon_512x512";

        if (argDic.ContainsKey("xiaomi") && argDic["xiaomi"] == "true")
        {
            iconName = "icon_512x512_xiaomi";
        }

        Texture2D texture = AssetDatabase.LoadAssetAtPath(string.Format("Assets/Resources/{0}.png", iconName),
            typeof(Texture2D)) as Texture2D;

        int[] iconSize = PlayerSettings.GetIconSizesForTargetGroup(BuildTargetGroup.Android);
        Texture2D[] textureArray = new Texture2D[iconSize.Length];
        for (int i = 0; i < textureArray.Length; i++)
        {
            textureArray[i] = texture;
        }
        textureArray[0] = texture;
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, textureArray);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 官网包拷贝支付宝文件
    /// </summary>
    //[MenuItem("Tools/TestCopyRes")]
    static void CopyAndroidRes()
    {
        //支付宝aar
        string aar_path = Application.dataPath + "/Plugins/Android/alipaySdk-15.5.9-20181123210601.aar";
        if (File.Exists(aar_path))
        {
            File.Delete(aar_path);
        }

        //Android Plugin
        string plugin_aar_path = Application.dataPath + "/Plugins/Android/app-debug.aar";
        if (File.Exists(plugin_aar_path))
        {
            File.Delete(plugin_aar_path);
        }

        //修改AndroidManifest文件
        string androidmanifest_path = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
        if (File.Exists(androidmanifest_path))
        {
            File.Delete(androidmanifest_path);
        }

        if (argDic.ContainsKey("anysdk") && argDic["anysdk"] == "true")
        {
            //app-debug.aar
            if (argDic.ContainsKey("ysdk") && argDic["ysdk"] == "true")
            {
                //应用宝
                string plugin_source_path = Application.dataPath + "/../SpecialRes/ysdk_aar/app-debug.aar";
                File.Copy(plugin_source_path, plugin_aar_path);
            }
            else
            {
                //其他渠道
                string plugin_source_path = Application.dataPath + "/../SpecialRes/common_aar/app-debug.aar";
                File.Copy(plugin_source_path, plugin_aar_path);
            }

            //AndroidManifest
            string source_path = Application.dataPath + "/../SpecialRes/androidmanifest/anysdk/AndroidManifest.xml";
            File.Copy(source_path, androidmanifest_path);
        }
        else if (argDic.ContainsKey("hwsdk") && argDic["hwsdk"] == "true")
        {
            //华为
            string plugin_source_path = Application.dataPath + "/../SpecialRes/huawei/app-debug.aar";
            File.Copy(plugin_source_path, plugin_aar_path);

            string manifest_source_path = Application.dataPath + "/../SpecialRes/huawei/AndroidManifest.xml";
            File.Copy(manifest_source_path, androidmanifest_path);

            // 删除无用的
            string anysdk_path_1 = Application.dataPath + "/Plugins/Android/libs/armeabi-v7a/libPluginProtocol.so";
            if (File.Exists(anysdk_path_1))
                File.Delete(anysdk_path_1);
            string anysdk_path_2 = Application.dataPath + "/Plugins/Android/libs/x86/libPluginProtocol.so";
            if (File.Exists(anysdk_path_2))
                File.Delete(anysdk_path_2);
            string anysdk_path_3 = Application.dataPath + "/Plugins/Android/libs/libPluginProtocolForUnity_fat.jar";
            if (File.Exists(anysdk_path_3))
                File.Delete(anysdk_path_3);
        }
        else
        {
            string plugin_source_path = Application.dataPath + "/../SpecialRes/gw_aar/app-debug.aar";
            File.Copy(plugin_source_path, plugin_aar_path);

            //官网包
            string source_path = Application.dataPath + "/../SpecialRes/alipaySdk-15.5.9-20181123210601.aar";
            File.Copy(source_path, aar_path);

            string manifest_source_path = Application.dataPath + "/../SpecialRes/androidmanifest/gw/AndroidManifest.xml";
            File.Copy(manifest_source_path, androidmanifest_path);
        }
    }
}
