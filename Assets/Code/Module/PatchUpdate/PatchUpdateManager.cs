
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class PatchVersion
{
    public int EngineVersion;
    public int ResVersion;
    public string PatchResPath;
}

[Serializable]
public class PatchRes
{
    public List<PatchResInfo> resList;
}

[Serializable]
public class PatchResInfo
{
    public string resName;
    public string resMD5;
    public long resLength;
}
    

class ResourceUpdateManager : SingletonMono<ResourceUpdateManager>
{
    private PatchVersion mPatchVersionLocal;
    private PatchVersion mPatchVersionServer;
    private PatchRes mPatchResLocal;
    private PatchRes mPatchResServer;

    public void CheckUpdate()
    {
        StartCoroutine(CoCheckUpdate());
    }

    public IEnumerator CoCheckUpdate()
    {
        yield return InitPatchData();
    }

    IEnumerator InitPatchData()
    {
        //获取本地的path res信息
        string persistentPatchResPath = PathManager.RES_PERSISTENT_ROOT_PATH + "/" + PathManager.GetRuntimePlatform() + "/PatchRes.text";
        if(File.Exists(persistentPatchResPath))
        {
            using (FileStream fs = File.Open(persistentPatchResPath, FileMode.Open, FileAccess.ReadWrite))
            using (StreamReader sr = new StreamReader(fs))
            {
                string content = sr.ReadToEnd();
                mPatchResLocal = JsonUtility.FromJson<PatchRes>(content);
            }
        }
        else
        {
            string streamPatchResPath = PathManager.RES_STREAM_ROOT_PATH + "/" + PathManager.GetRuntimePlatform() + "/PatchRes.text";
            using (UnityWebRequest request = new UnityWebRequest(streamPatchResPath))
            {
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    Debuger.LogError("common", request.error);
                }
                else
                {
                    using (FileStream fs = File.Open(persistentPatchResPath, FileMode.CreateNew, FileAccess.ReadWrite))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        string content = request.downloadHandler.text;
                        sw.Write(content);
                        sw.Flush();
                        mPatchResLocal = JsonUtility.FromJson<PatchRes>(content);
                    }
                }
            }
        }

        //获取本地的path version信息
        string persistentPatchVersionPath = PathManager.RES_PERSISTENT_ROOT_PATH + "/" + PathManager.GetRuntimePlatform() + "/PatchVersion.text";
        if (File.Exists(persistentPatchVersionPath))
        {
            using (FileStream fs = File.Open(persistentPatchVersionPath, FileMode.Open, FileAccess.ReadWrite))
            using (StreamReader sr = new StreamReader(fs))
            {
                string content = sr.ReadToEnd();
                mPatchVersionLocal = JsonUtility.FromJson<PatchVersion>(content);
            }
        }
        else
        {
            string streamPatchVersionPath = PathManager.RES_STREAM_ROOT_PATH + "/" + PathManager.GetRuntimePlatform() + "/PatchVersion.text";
            using (UnityWebRequest request = new UnityWebRequest(streamPatchVersionPath))
            {
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    Debuger.LogError("common", request.error);
                }
                else
                {
                    using (FileStream fs = File.Open(persistentPatchVersionPath, FileMode.CreateNew, FileAccess.ReadWrite))
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        string content = request.downloadHandler.text;
                        sw.Write(content);
                        sw.Flush();
                        mPatchVersionLocal = JsonUtility.FromJson<PatchVersion>(content);
                    }
                }
            }
        }

        //获取服务器的patch version信息
        string serverPatchVersionPath = PathManager.GetServerURL("PatchVersion.text");
        using (UnityWebRequest request = new UnityWebRequest(serverPatchVersionPath))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debuger.LogError("common", request.error);
            }
            else
            {
                string content = request.downloadHandler.text;
                mPatchVersionServer = JsonUtility.FromJson<PatchVersion>(content);
            }
        }

        //更新APK
        if (mPatchVersionLocal.EngineVersion < mPatchVersionServer.EngineVersion)
        {
            Application.Quit();
            yield break;
        }

        //更新资源
        if (mPatchVersionLocal.ResVersion < mPatchVersionServer.ResVersion)
        {
            string serverPatchResPath = PathManager.GetServerURL(mPatchVersionServer.PatchResPath + "/PatchRes.text");
            using (UnityWebRequest request = new UnityWebRequest(serverPatchResPath))
            {
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    Debuger.LogError("common", request.error);
                }
                else
                {
                    string content = request.downloadHandler.text;
                    mPatchResServer = JsonUtility.FromJson<PatchRes>(content);
                }
            }

            yield return StartCoroutine(StartUpdateRes());

            //更新批次数据
            UpdatePatchData();
        }
        else if(mPatchVersionLocal.ResVersion > mPatchVersionServer.ResVersion)
        {
            Debuger.LogError("common", "mPatchVersionLocal.ResVersion > mPatchVersionServer.ResVersion");
        }
    }

    public IEnumerator StartUpdateRes()
    {
        List<PatchResInfo> updateList = new List<PatchResInfo>();
        for (int i = 0; i < mPatchResServer.resList.Count; i++)
        {
            string resName = mPatchResServer.resList[i].resName;
            PatchResInfo resData = mPatchResLocal.resList.Find((item) => { return item.resName == resName; });
            if (resData != null)
            {
                if (resData.resMD5 != mPatchResServer.resList[i].resMD5)
                {
                    updateList.Add(mPatchResServer.resList[i]);
                    resData.resMD5 = mPatchResServer.resList[i].resMD5;
                }
            }
        }

        //开始加载资源
        for (int i = 0; i < updateList.Count; i++)
        {
            //首先要判断这个文件是否存在，并且MD5码是否匹配，来判断是否下载这个文件。（因为这个文件可能没写入完成，用户就退出了，有这种情况）
            string resPath = string.Format("{0}/{1}/{2}", PathManager.RES_PERSISTENT_ROOT_PATH, PathManager.GetRuntimePlatform(), updateList[i].resName);
            bool isLoad = false;
            if (File.Exists(resPath))
            {
                string md5 = FileUtility.MD5File(resPath);
                if (md5 != updateList[i].resMD5)
                {
                    isLoad = true;
                    File.Delete(resPath);
                }
            }
            else
            {
                isLoad = true;
            }

            if (isLoad)
            {
                using (UnityWebRequest request = new UnityWebRequest(PathManager.GetServerURL(mPatchVersionServer.ResVersion + "/" + updateList[i].resName)))
                {
                    yield return request;
                    string dirPath = Path.GetDirectoryName(resPath);
                    FileUtility.CreateDirectory(dirPath);
                    using (FileStream fs = File.Open(resPath, FileMode.Create, FileAccess.ReadWrite))
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(request.downloadHandler.data);
                        bw.Flush();
                    }
                }
            }
        }
    }

    void UpdatePatchData()
    {
        //更新patch version
        string persistentPatchVersionPath = PathManager.RES_PERSISTENT_ROOT_PATH + "/" + PathManager.GetRuntimePlatform() + "/PatchVersion.text";
        using (FileStream fs = File.Open(persistentPatchVersionPath, FileMode.Create, FileAccess.ReadWrite))
        using (StreamWriter sw = new StreamWriter(fs))
        {
            string content = JsonUtility.ToJson(mPatchVersionServer);
            sw.Write(content);
            sw.Flush();
        }

        //更新patch res
        string persistentPatchResPath = PathManager.RES_PERSISTENT_ROOT_PATH + "/" + PathManager.GetRuntimePlatform() + "/PatchRes.text";
        using (FileStream fs = File.Open(persistentPatchResPath, FileMode.Create, FileAccess.ReadWrite))
        using (StreamWriter sw = new StreamWriter(fs))
        {
            string content = JsonUtility.ToJson(mPatchVersionLocal);
            sw.Write(content);
            sw.Flush();
        }
    }

    ////检测更新是否中断
    //bool CheckUpdateInterrupt()
    //{
    //    string path = PathManager.RES_PERSISTENT_ROOT_PATH + "/" + PathManager.GetRuntimePlatform() + "/MarkUpdate.text";
    //    bool isExist = File.Exists(path);
    //    return isExist;
    //}
    //
    ////标记更新开始
    //void MarkUpdateStart()
    //{
    //    string path = PathManager.RES_PERSISTENT_ROOT_PATH + "/" + PathManager.GetRuntimePlatform() + "/MarkUpdate.text";
    //    if(!File.Exists(path))
    //    {
    //        File.Open(path, FileMode.Create);
    //    }
    //}
    //
    ////标记更新结束
    //void MarkUpdateEnd()
    //{
    //    string path = PathManager.RES_PERSISTENT_ROOT_PATH + "/" + PathManager.GetRuntimePlatform() + "/MarkUpdate.text";
    //    if (File.Exists(path))
    //    {
    //        File.Delete(path);
    //    }
    //}

    //清理Persistent目录
    void ClearPersistentDir()
    {

    }

    //检测网络
    bool CheckNetwork()
    {
        return false;
    }

    //检测是否是新包
    bool CheckIsNewPackage()
    {
        return false;
    }

    //检测是否要重新安装APK
    bool CheckIsNeedReinstallAPK()
    {
        return false;
    }

    //检测是否更新新资源
    bool CheckIsUpdateRes()
    {
        return false;
    }


}
