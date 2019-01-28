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
        BuildAB.
        //将你打包的资源拷贝到StreamingAssets中
        //FileUtility.CopyTo(Application.dataPath + "/../../ClientRes/IOS", Application.dataPath + "/StreamingAssets/ClientRes/IOS");
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
    //这个函数会在BuildPipeline.BuildPlayer完成后调用，用于修改xcode工程的属性
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
}
