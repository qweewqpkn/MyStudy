using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BuildRes
{
    [MenuItem("Tools/AssetBundle/打包所有资源")]
    public static void Build()
    {
        BuildABAdd();
        BuildLua();
    }

    [MenuItem("Tools/AssetBundle/增量打包AB")]
    public static void BuildABAdd()
    {
        BuildAB(false);
    }

    [MenuItem("Tools/AssetBundle/重新打包AB")]
    public static void BuildABAll()
    {
        BuildAB(true);
    }

    static void BuildAB(bool isALL)
    {
        SetAllABName();
        string path = GetRootPath(RootPathType.eLocal, "Assetbundle");
        if(isALL)
        {
            if(Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
        else
        {
            RemoveUnUseAB(path);
        }
        FileUtility.CreateDirectory(path);
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        Debug.Log("打包完成");
    }

    [MenuItem("Tools/AssetBundle/打包Lua")]
    static void BuildLua()
    {
        //从lua原始目录拷贝到目标目录并加上.bytes后缀
        string source_path = Application.dataPath + "/Lua";
        string target_path = Application.dataPath + "/LuaTemp";
        FileUtility.CopyTo(source_path, target_path, "*.lua", "bytes", target_path);
        AssetDatabase.Refresh();

        //以目标lua目录下的文件作为模块，打包成不同的AB中
        Dictionary<string, List<string>> abbDict = new Dictionary<string, List<string>>();
        string[] Directories = Directory.GetDirectories(target_path, "*");
        for(int i = 0; i < Directories.Length; i++)
        {
            //模块名
            string moduleName = Directories[i].Replace("\\", "/").Replace(target_path + "/", "").Split('/')[0];
            if(!abbDict.ContainsKey(moduleName))
            {
                abbDict[moduleName] = new List<string>();
            }

            //文件列表
            string[] files = Directory.GetFiles(Directories[i], "*.bytes", SearchOption.AllDirectories);
            for(int j = 0; j < files.Length; j++)
            {
                abbDict[moduleName].Add(files[j].Substring(files[j].IndexOf("Assets")));
            }
        }

        //LuaTemp下的目录名将作为模块名，而各个目录下的文件将会打包到对应模块AB中
        List<AssetBundleBuild> abbList = new List<AssetBundleBuild>();
        foreach(var item in abbDict)
        {
            AssetBundleBuild abb = new AssetBundleBuild();
            abb.assetBundleName = item.Key;
            abb.assetNames = item.Value.ToArray();
            abbList.Add(abb);
        }

        string output_path = GetRootPath(RootPathType.eLocal, "Lua");
        if(Directory.Exists(output_path))
        {
            Directory.Delete(output_path, true);
        }
        FileUtility.CreateDirectory(output_path);
        BuildPipeline.BuildAssetBundles(output_path, abbList.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
    }

    static void MD5Res()
    {
        string path = GetRootPath(RootPathType.eLocal);
        string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        StringBuilder sb = new StringBuilder();
        VersionRes version = new VersionRes();
        version.version = "1.00";
        version.resList = new List<VersionResData>(); 
        for(int i = 0; i < files.Length; i++)
        {
            VersionResData resData = new VersionResData();
            resData.resLength = FileUtility.GetFileLength(files[i]);
            resData.resMD5 = FileUtility.MD5File(files[i]);
            resData.resName = files[i].Replace("\\", "/").Replace(path + "/", "");
            version.resList.Add(resData);
            //sb.AppendLine(string.Format("{0}|{1}|{2}", files[i].Replace("\\", "/").Replace(path + "/", ""), hash, fs.Length));
        }

        string versionJson = JsonUtility.ToJson(version);
        FileStream md5FS = File.Open(path + "/VersionRes.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(md5FS);
        sw.Write(versionJson);
        sw.Close();
        md5FS.Close();
        md5FS.Dispose();

        Debug.Log("生成MD5成功!");
    }

    public static void CopyResToStreamingAssets()
    {
        if(EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64 && EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows)
        {
            FileUtility.CopyTo(GetRootPath(RootPathType.eLocal), GetRootPath(RootPathType.ePhone));
            AssetDatabase.Refresh();
            Debug.Log("拷贝资源成功!");
        }
    }

    //移除无用的AB
    static void RemoveUnUseAB(string path)
    {
        if(!Directory.Exists(path))
        {
            return;
        }

        string[] curABNameList = AssetDatabase.GetAllAssetBundleNames();
        string[] unuseABNameList = AssetDatabase.GetUnusedAssetBundleNames();
        for (int i = 0; i < unuseABNameList.Length; i++)
        {
            ArrayUtility.Remove(ref curABNameList, unuseABNameList[i]);
        }
        string[] buildABList = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        for (int i = 0; i < buildABList.Length; i++)
        {
            string name = buildABList[i].Replace("\\", "/").Replace(path + "/", "").Split('.')[0];
            bool isHave = ArrayUtility.Contains(curABNameList, name);
            if (!isHave)
            {
                File.Delete(buildABList[i]);
                Debug.Log("delete unuse ab : " + name);
            }
        }

        FileUtility.ClearEmptyDirectory(path);
    }

    //设置AB的名字
    static void SetAllABName()
    {
        string[] files = Directory.GetFiles(PathManager.RES_EXPORT_ROOT_PATH, "*.*", SearchOption.AllDirectories);

        if (files != null)
        {
            Dictionary<string, string> resMap = new Dictionary<string, string>();
            for (int i = 0; i < files.Length; i++)
            {
                if (resMap.ContainsKey(files[i]))
                {
                    Debug.LogError("重名资源,请检测!路径 : " + files[i]);
                    return;
                }
                else
                {
                    resMap.Add(files[i], files[i]);
                }
            }

            for (int i = 0; i < files.Length; i++)
            {
                string ext = Path.GetExtension(files[i]);
                if(ext != ".meta")
                {
                    if(files[i].Contains("Shader"))
                    {
                        AssetImporter ai = AssetImporter.GetAtPath(files[i]);
                        ai.assetBundleName = "allshader";
                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(files[i]);
                        AssetImporter ai = AssetImporter.GetAtPath(files[i]);                   
                        ai.assetBundleName = fileName;
                    }
                }
            }
        }
        else
        {
            Debug.LogError(string.Format("目录{0}资源为空,请确认!", PathManager.RES_EXPORT_ROOT_PATH));
;       }
    }

    //重置AB的名字
    static void ResetAllABName()
    {
        string[] files = Directory.GetFiles(PathManager.RES_EXPORT_ROOT_PATH, "*.*", SearchOption.AllDirectories);
        if (files != null)
        {
            for (int i = 0; i < files.Length; i++)
            {
                string ext = Path.GetExtension(files[i]);
                if (ext != ".meta")
                {
                    AssetImporter ai = AssetImporter.GetAtPath(files[i]);
                    ai.assetBundleName = "";
                }
            }
        }
    }


    public enum RootPathType
    {
        eLocal,
        ePhone,
    }
    public static string GetRootPath(RootPathType type, string name = "")
    {
        StringBuilder result = new StringBuilder();
        switch (type)
        {
            case RootPathType.eLocal:
                {
                    result.Append(PathManager.RES_LOCAL_ROOT_PATH);
                }
                break;
            case RootPathType.ePhone:
                {
                    result.Append(PathManager.RES_STREAM_ROOT_PATH);
                }
                break;
        }
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                {
                    result.Append("/Android");
                }
                break;
            case BuildTarget.iOS:
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            //case BuildTarget.StandaloneOSXUniversal:
                {
                    result.Append("/IOS");
                }
                break;
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneWindows:
                {
                    result.Append("/Windows");
                }
                break;
        }

        if (!string.IsNullOrEmpty(name))
        {
            result.Append("/");
            result.Append(name);
        }
        return result.ToString();
    }
}
