using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[Serializable]
public class VersionRes
{
    public string version;
    public List<VersionResData> resList;
}

[Serializable]
public class VersionResData
{
    public string resName;
    public string resMD5;
    public long resLength;
}

[Serializable]
public class UpdataHistory
{
    public List<UpdataHistoryItem> dic;
}
[Serializable]
public class UpdataHistoryItem
{
    public string name;
    public string opt;
}

public class BuildRes : EditorWindow
{
    public enum WindowType
    {
        eNone,
        eSetABName,
        eBuildDiff,
    }

    public static WindowType mWindowType = WindowType.eNone;

    [MenuItem("Tools/AssetBundle/增量打包(开发)", false, 500)]
    public static void BuildAdd()
    {
        BuildAB(false);
        BuildLua();
        Debug.Log(string.Format("{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now));
    }

    [MenuItem("Tools/AssetBundle/重新打包(开发)", false, 501)]
    public static void BuildAll()
    {
        BuildAB(true);
        BuildLua();
        Debug.Log(string.Format("{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now));
    }

    [MenuItem("Tools/AssetBundle/增量打包(正式)", false, 502)]
    public static void BuildAddOfficial()
    {
        BuildAB(false);
        BuildLua();
        MD5Res();
        GeneratorDiff();
        Debug.Log(string.Format("{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now));
    }

    [MenuItem("Tools/AssetBundle/重新打包(正式)", false, 503)]
    public static void BuildAllOfficial()
    {
        BuildAB(true);
        BuildLua();
        MD5Res();
        GeneratorDiff();
        Debug.Log(string.Format("{0:yyyy-MM-dd-HH-mm-ss-fff}", DateTime.Now));
    }

    static void BuildAB(bool isALL)
    {
        bool isOk = SetAllABName();
        if(isOk)
        {
            string path = PathManager.RES_LOCAL_ROOT_PATH + "/" + PathManager.GetEditorPlatform() + "/" + "Assetbundle";
            if (isALL)
            {
                if (Directory.Exists(path))
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
            Debug.Log("打包资源完成");
        }
        else
        {
            Debug.Log("打包资源失败");
        }
    }

    static void BuildLua()
    {
        //从lua原始目录拷贝到目标目录并加上.bytes后缀
        string source_path = PathManager.LUA_ROOT_PATH;
        string target_path = Application.dataPath + "/LuaTemp";
        string rootPath = target_path.Replace("\\", "/");
        FileUtility.CopyTo(source_path, target_path, "*.lua", "bytes", (targetPath, filePath)=>
        {
            string fileName = "";
            if (targetPath == rootPath)
            {
                fileName = Path.GetFileName(filePath);
            }
            else
            {
                string moduleName = targetPath.Replace(rootPath + "/", "").Split('/')[0]; //取根目录下的文件名作为模块名
                fileName = targetPath.Replace(rootPath + "/" + moduleName, ""); //获取模块下文件的相对路径
                                                                                       //fileName可能为空，在当前模块目录下有文件时
                if (!string.IsNullOrEmpty(fileName))
                {
                    //移除斜杠,替换斜杠
                    fileName = fileName.Remove(0, 1).Replace("/", "_");
                    fileName = fileName + "_" + Path.GetFileName(filePath);
                }
                else
                {
                    fileName = Path.GetFileName(filePath);
                }
            }

            return fileName;
        });
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

        string output_path = PathManager.RES_LOCAL_ROOT_PATH + "/" + PathManager.GetEditorPlatform() + "/" + "Lua";
        if (Directory.Exists(output_path))
        {
            Directory.Delete(output_path, true);
        }
        FileUtility.CreateDirectory(output_path);
        BuildPipeline.BuildAssetBundles(output_path, abbList.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        Debug.Log("打包Lua完成");
    }

    static void MD5Res()
    {
        string path = PathManager.RES_LOCAL_ROOT_PATH + "/" + PathManager.GetEditorPlatform();
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
            if(resData.resName != "VersionRes.txt")
            {
                version.resList.Add(resData);
            }
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
                //Debug.Log("delete unuse ab : " + name);
            }
        }

        FileUtility.ClearEmptyDirectory(path);
    }

    static string GetABName(string path)
    {
        string abName = "";
        path = path.Replace("\\", "/");
        if(path.Contains("Export/Merge/")) //合并目录，取文件的父目录名字作为AB名
        {
            string[] strList = path.Split('/');
            if(strList.Length >= 2)
            {
                abName = strList[strList.Length - 2];
            }
            else
            {
                Debug.LogError("GetABName Error , Path : " + path);
            }
        }
        else //其余文件,直接取自身名字就可以了
        {
            abName = Path.GetFileNameWithoutExtension(path);
        }

        if(string.IsNullOrEmpty(abName))
        {
            Debug.LogError("abName is empty Path : " + path);
        }

        return abName;
    }

    //设置AB的名字
    static bool SetAllABName()
    {
        string[] files = Directory.GetFiles(PathManager.RES_EXPORT_ROOT_PATH, "*.*", SearchOption.AllDirectories);

        if (files != null)
        {
            Dictionary<string, string> resMap = new Dictionary<string, string>();
            for (int i = 0; i < files.Length; i++)
            {
                string ext = Path.GetExtension(files[i]);
                if (ext != ".meta")
                {
                    string abName = GetABName(files[i]);
                    if (resMap.ContainsKey(abName))
                    {
                        if(!files[i].Replace("\\", "/").Contains("Export/Merge/"))
                        {
                            Debug.LogError(string.Format("重名资源{0},请检测Export下的文件!", abName));
                            return false;
                        }
                    }
                    else
                    {
                        resMap.Add(abName, abName);
                    }
                }
            }

            for (int i = 0; i < files.Length; i++)
            {
                string ext = Path.GetExtension(files[i]);
                files[i] = files[i].Replace("\\", "/");
                if (ext != ".meta")
                {
                    if(files[i].Contains("Shader"))
                    {
                        AssetImporter ai = AssetImporter.GetAtPath(files[i]);
                        ai.assetBundleName = "allshader";
                    }
                    else if(files[i].Contains("Font"))
                    {
                        AssetImporter ai = AssetImporter.GetAtPath(files[i]);
                        ai.assetBundleName = "font";
                    }
                    else if(files[i].Contains("Export/Merge/")) //这个目录下的东西，按照子目录作为一个AB
                    {
                        AssetImporter ai = AssetImporter.GetAtPath(files[i]);
                        ai.assetBundleName = GetABName(files[i]);
                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(files[i]);
                        AssetImporter ai = AssetImporter.GetAtPath(files[i]);                   
                        ai.assetBundleName = fileName;
                    }
                }
            }

            return true;
        }
        else
        {
            Debug.LogError(string.Format("目录{0}资源为空,请确认!", PathManager.RES_EXPORT_ROOT_PATH));
            return false;
;       }
    }

    //重置AB的名字
    [MenuItem("Tools/AssetBundle/清空AB名字", false, 600)]
    static void ResetAllABName()
    {
        string[] files = Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories);
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

    [MenuItem("Tools/AssetBundle/设置AB名字", false, 601)]
    private static void SetABNameWindow()
    {
        mWindowType = WindowType.eSetABName;
        EditorWindow window = EditorWindow.GetWindow(typeof(BuildRes));
        window.Show();
    }

    private string mABName = "";
    private static void SetABName(string name)
    {
        if (Selection.objects != null)
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                string assetPath = AssetDatabase.GetAssetPath(Selection.objects[i]);
                AssetImporter ai = AssetImporter.GetAtPath(assetPath);
                ai.assetBundleName = name;
            }
        }
    }

    private void OnGUI()
    {
        if(mWindowType == WindowType.eSetABName)
        {
            mABName = EditorGUILayout.TextField("AB名字", mABName);
            if (GUILayout.Button("应用AB名字"))
            {
                SetABName(mABName);
            }
        }
        else if(mWindowType == WindowType.eBuildDiff)
        {
            if (GUILayout.Button("选择资源路径1"))
            {
                mDiffResPath1 = EditorUtility.OpenFolderPanel("选择文件", mOfficalPath, "");
            }
            mDiffResPath1 = EditorGUILayout.TextField("资源路径1", mDiffResPath1);
            if (GUILayout.Button("选择资源路径2"))
            {
                mDiffResPath2 = EditorUtility.OpenFolderPanel("选择文件", mOfficalPath, "");
            }
            mDiffResPath2 = EditorGUILayout.TextField("资源路径2", mDiffResPath2);
            if (GUILayout.Button("选择生成差异文件路径"))
            {
                mOutputDiffPath = EditorUtility.OpenFolderPanel("选择文件", mOfficalPath, "");
            }
            mOutputDiffPath = EditorGUILayout.TextField("生成差异文件路径", mOutputDiffPath);
            if (GUILayout.Button("选择历史差异文件路径"))
            {
                mOutputHistoryPath = EditorUtility.OpenFolderPanel("选择文件", mOfficalPath, "");
            }
            mOutputHistoryPath = EditorGUILayout.TextField("历史差异文件路径", mOutputHistoryPath);
            if (GUILayout.Button("生成差异文件"))
            {
                GeneratorDiff();
            }
        }
    }

    private static string mDiffResPath1 = "";
    private static string mDiffResPath2 = "";
    private static string mOutputDiffPath = "";
    private static string mOutputHistoryPath = "";
    private static string mOfficalPath = "";
    [MenuItem("Tools/AssetBundle/生成差异文件", false, 700)]
    public static void GeneratorDiffWindow()
    {
        //mOfficalPath = PathManager.RES_LOCAL_ROOT_PATH + "/" + PathManager.GetEditorPlatform() + "_offical";
        mOfficalPath = @"E:\ProjectAAndroid\Branches\Client\MainHotUpdate\ClientRes\Android_offical";
        //mOfficalPath = @"E:\Project\ProjectA\Branches\Client\Main\ClientRes\Windows_offical";
        mWindowType = WindowType.eBuildDiff;
        EditorWindow window = EditorWindow.GetWindow(typeof(BuildRes));
        window.Show();
    }

    private static void GeneratorDiff()
    {
        StringBuilder result = new StringBuilder();
        string path1 = mDiffResPath1 + "/VersionRes.txt";
        string path2 = mDiffResPath2 + "/VersionRes.txt";
        if (!File.Exists(path1) || !File.Exists(path2))
        {
            Debug.LogError("GeneratorDiff 找不到目标文件");
            return;
        }
        string content1 = File.ReadAllText(path1);
        string content2 = File.ReadAllText(path2);
        VersionRes data1 = JsonUtility.FromJson<VersionRes>(content1);
        VersionRes data2 = JsonUtility.FromJson<VersionRes>(content2);

        ReadAllHistory(mOutputHistoryPath);

        string templateStr = "<file path=\"{0}\" opt=\"{1}\" size=\"{2}\"/>";
        result.AppendLine("<root>");
        for (int i = 0; i < data1.resList.Count; i++)
        {
            VersionResData sourceData = data1.resList[i];
            VersionResData targetData = data2.resList.Find((VersionResData resData) =>
            {
                if (resData.resName == sourceData.resName)
                {
                    return true;
                }

                return false;
            });

            if (targetData != null)
            {
                if (sourceData.resMD5 != targetData.resMD5)
                {
                    result.AppendLine(string.Format(templateStr, targetData.resName, GetHistory(targetData.resName, "update"), targetData.resLength));
                    FileCopyToDir(targetData.resName, mDiffResPath2, mOutputDiffPath);
                }
            }
            else
            {
                result.AppendLine(string.Format(templateStr, sourceData.resName, GetHistory(sourceData.resName, "delete"), sourceData.resLength));
            }
        }

        for (int i = 0; i < data2.resList.Count; i++)
        {
            VersionResData sourceData = data2.resList[i];
            VersionResData targetData = data1.resList.Find((VersionResData resData) =>
            {
                if (resData.resName == sourceData.resName)
                {
                    return true;
                }

                return false;
            });

            if (targetData == null)
            {
                result.AppendLine(string.Format(templateStr, sourceData.resName, GetHistory(sourceData.resName, "add"), sourceData.resLength));
                FileCopyToDir(sourceData.resName, mDiffResPath2, mOutputDiffPath);
            }
        }
        result.AppendLine("</root>");

        FileStream fs = File.Open(mOutputDiffPath + "/filemanifest.xml", FileMode.Create, FileAccess.ReadWrite);
        StreamWriter wr = new StreamWriter(fs);
        wr.Write(result);
        wr.Close();
        fs.Close();

        SaveAllHistory(mOutputHistoryPath);

        //ZipUtility.ZipFileFromDirectory(mOutputDiffPath, mOutputDiffPath + ".zip", 4);
        //ZipUtil.ExeCompOne(mOutputDiffPath, mOutputDiffPath + ".zip");

        Debug.Log("############Finish");
    }

    public static UpdataHistory history;
    public static Dictionary<string, string> historDic;
    public static void ReadAllHistory(string path)
    {
        string path1 = path + "/UpdateHistory.txt";
        if (File.Exists(path1))
        {
            string content1 = File.ReadAllText(path1);
            history = JsonUtility.FromJson<UpdataHistory>(content1);
        }
        else
        {
            history = new UpdataHistory();
            history.dic = new List<UpdataHistoryItem>();
        }

        historDic = new Dictionary<string, string>();
        for (int i = 0; i < history.dic.Count; i++)
        {
            historDic[history.dic[i].name] = history.dic[i].opt;
        }
    }
    public static string GetHistory(string resName, string opt)
    {
        if(opt == "delete")
        {
            if (historDic.ContainsKey(resName))
            {
                historDic.Remove(resName);
            }
            return opt;
        }
        else
        {
            if (historDic.ContainsKey(resName))
            {
                historDic[resName] = "update";
                return "update";
            }
            else
            {
                historDic.Add(resName, "add");
                return "add";
            }
        }
    }
    public static void SaveAllHistory(string path)
    {
        history.dic.Clear();
        foreach (var item in historDic)
        {
            UpdataHistoryItem itemList = new UpdataHistoryItem();
            itemList.name = item.Key;
            itemList.opt = item.Value;
            history.dic.Add(itemList);
        }
        string versionJson = JsonUtility.ToJson(history);
        FileStream md5FS = File.Open(path + "/UpdateHistory.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(md5FS);
        sw.Write(versionJson);
        sw.Close();
        md5FS.Close();
        md5FS.Dispose();
    }

    static void FileCopyToDir(string res, string source_path, string targetDir)
    {
        string dir = targetDir + "/" + res.Remove(res.LastIndexOf('/'));
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        File.Copy(source_path + "/" + res, targetDir + "/" + res);
    }
}
