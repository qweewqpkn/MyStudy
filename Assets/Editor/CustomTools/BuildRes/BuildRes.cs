using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class BuildRes
{
    [MenuItem("Tools/AssetBundle/打包资源")]
    public static void Build()
    {
        BuildABAll();
        BuildLua();
        MD5Res();
        CopyResToStreamingAssets();
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

    [MenuItem("Tools/打包Lua")]
    static void BuildLua()
    {
        FileUtility.CopyTo(Application.dataPath + "/Lua", Application.dataPath + "/LuaTemp", "*.lua", "bytes", Application.dataPath + "/LuaTemp");
        FileUtility.CopyTo(Application.dataPath + "/ToLua/Lua", Application.dataPath + "/LuaTemp/ToLua", "*.lua", "bytes", Application.dataPath + "/LuaTemp");
        AssetDatabase.Refresh();

        //获取lua根目录下的各个模块名和对应模块的lua文件
        string luaRootPath = Application.dataPath + "/LuaTemp";
        Dictionary<string, List<string>> abbDict = new Dictionary<string, List<string>>();
        string[] Directories = Directory.GetDirectories(luaRootPath, "*");
        for(int i = 0; i < Directories.Length; i++)
        {
            //模块名
            string moduleName = Directories[i].Replace("\\", "/").Replace(luaRootPath + "/", "").Split('/')[0];
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

        string path = GetRootPath(RootPathType.eLocal,"Lua");
        if(Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        FileUtility.CreateDirectory(path);
        BuildPipeline.BuildAssetBundles(path, abbList.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
    }

    //移除无用的AB
    static void RemoveUnUseAB(string path)
    {
        string[] curABNameList = AssetDatabase.GetAllAssetBundleNames();
        string[] unuseABNameList = AssetDatabase.GetUnusedAssetBundleNames();
        for(int i = 0; i < unuseABNameList.Length; i++)
        {
            ArrayUtility.Remove(ref curABNameList, unuseABNameList[i]);
        }
        string[] buildABList = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        for(int i = 0; i < buildABList.Length; i++)
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

    [MenuItem("Tools/生成资源MD5")]
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

    [MenuItem("Tools/AssetBundle/拷贝资源到StreamingAssets")]
    public static void CopyResToStreamingAssets()
    {
        if(EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64 && EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows)
        {
            FileUtility.CopyTo(GetRootPath(RootPathType.eLocal), GetRootPath(RootPathType.ePhone));
            AssetDatabase.Refresh();
            Debug.Log("拷贝资源成功!");
        }
    }

    [MenuItem("Tools/AssetBundle/设置所有AB Name")]
    static void SetAllABName()
    {
        string[] files = Directory.GetFiles(PathManager.RES_EXPORT_ROOT_PATH, "*.*", SearchOption.AllDirectories);
        if(files != null)
        {
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

    [MenuItem("Tools/AssetBundle/重置所有AB Name")]
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
            case BuildTarget.StandaloneOSX:
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
