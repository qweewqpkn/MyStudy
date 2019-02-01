using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AutoBuildIOS {

    protected static Dictionary<string, string> argDic = new Dictionary<string, string>();

    [MenuItem("Tools/打包IOS")]
    static void Build()
    {

    }

    static void BuildIOS()
    {
        InitArg();
        InitPlayerSetting();
        BuildAB.Build();
        FileUtility.CopyTo(PathManager.RES_PATH_IOS, PathManager.RES_PATH_IOS_PHONE);
        string[] level = GetBuildScene();
        string outputPath = argDic["output_path"].Replace("\\", "/");
        //生成唯一名字xcode工程,方便后面xcode自动化编译
        if (Directory.Exists(outputPath))
        {
            Directory.Delete(outputPath, true);
        }

        BuildPipeline.BuildPlayer(level, outputPath, BuildTarget.iOS, BuildOptions.None);
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

        StringBuilder defines = new StringBuilder();
        //根据外部传入参数添加指定宏
        if (GetArg("log") == "true")
        {
            defines.Append("LOG_OPEN;");
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defines.ToString());
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

#if UNITY_IOS
    //当BuildPipeline.BuildPlayer 完成后会调用这个函数，我们可以在这里修改xcode工程，根据需要添加库，修改xcode的证书还有各种属性
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

        //这里替换Replace("_", "-"),Replace("_", " ") 由于我们用了"-"作为分割识别符所以要改成了"_", 还有就是参数传递是已" "来分割的，所以当我们参数中自身就带了空格，那么就会截断，所以参数里面用"_"替换了" "，这里再替换回来
        project.SetBuildProperty(targetGUID, "PROVISIONING_PROFILE", argDic["PROVISIONING_PROFILE"].Replace("_", "-"));
        project.SetBuildProperty(targetGUID, "CODE_SIGN_IDENTITY", argDic["CODE_SIGN_IDENTITY"].Replace("_", " "));
        project.SetBuildProperty(targetGUID, "CODE_SIGN_IDENTITY[sdk=iphoneos*]", argDic["CODE_SIGN_IDENTITY"].Replace("_", " "));
        project.SetBuildProperty(targetGUID, "DEVELOPMENT_TEAM", argDic["DEVELOPMENT_TEAM"]);

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
        rootDict.SetString("CFBundleShortVersionString", argDic["Version"]);
        rootDict.SetString("CFBundleVersion", argDic["Build"]);

        pList.WriteToFile(plistPath);
    }
#endif
}
