﻿
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    

class ResourceUpdateManager : MonoBehaviour
{
    private static ResourceUpdateManager mInstance;
    public static ResourceUpdateManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "ResourceUpdateManager";
                mInstance = obj.AddComponent<ResourceUpdateManager>();
            }

            return mInstance;
        }
    }

    //检测资源更新
    public void CheckResUpdate()
    {
        //比对本地和服务器的资源描述文件中的几点
        //1.版本号，是大版本还是小版本，大版本要更新APK， 小版本更新资源即可
        //2.如果是小版本，那么我们要确认哪些资源是需要更新的，获得更新列表，进行更新
        StartCoroutine(LoadVersionRes());
    }

    public IEnumerator LoadVersionRes()
    {
        //获取服务器的version
        string serverVersionPath = PathManager.GetServerURL("VersionRes");
        WWW serverWWW = new WWW(serverVersionPath);
        yield return serverWWW;
        VersionRes serverVersionRes = JsonUtility.FromJson<VersionRes>(serverWWW.text);

        //获取streamingAssetsPath的version
        string streamVersionPath = PathManager.RES_STREAM_ROOT_PATH + PathManager.GetPlatform() + "/VersionRes.txt";
        WWW streamWWW = new WWW(streamVersionPath);
        yield return streamWWW;
        VersionRes streamVersionRes = JsonUtility.FromJson<VersionRes>(streamWWW.text);

        //获取persistentDataPath的version
        VersionRes persistentVersionRes = null;
        string persistentVersionPath = PathManager.RES_PERSISTENT_ROOT_PATH + PathManager.GetPlatform() + "/VersionRes.txt";
        if (File.Exists(persistentVersionPath))
        {
            string str = File.ReadAllText(persistentVersionPath);
            persistentVersionRes = JsonUtility.FromJson<VersionRes>(str);
        }

        int persistentVersion = 0;
        if (persistentVersionRes != null)
        {
            Int32.TryParse(persistentVersionRes.version.Replace(".", ""), out persistentVersion);
        }

        int streamVersion = 0;
        if(streamVersionRes != null)
        {
            Int32.TryParse(streamVersionRes.version.Replace(".", ""), out streamVersion);
        }

        int serverVersion = 0;
        if (serverVersionRes != null)
        {
            Int32.TryParse(serverVersionRes.version.Replace(".", ""), out serverVersion);
        }

        int localVersion = 0;
        VersionRes localVersionRes = null;
        if(persistentVersion != 0)
        {
            if (streamVersion > persistentVersion)
            {
                //热更新过，并且覆盖安装了apk
                localVersion = streamVersion;
                localVersionRes = streamVersionRes;
            }
            else
            {
                //热更新过
                localVersion = persistentVersion;
                localVersionRes = persistentVersionRes;
            }
        }
        else
        {
            //还未热更新过
            localVersion = streamVersion;
            localVersionRes = streamVersionRes;
        }
        
        if(serverVersion > localVersion)
        {
            Debug.Log("serverVersion > localVersion 需要热更新");
            StartCoroutine(StartUpdate(serverVersionRes, localVersionRes));
        }
        else if(serverVersion == localVersion)
        {
            Debug.Log("serverVersion == localVersion 无须热更新");
        }
        else
        {
            Debug.LogError("serverVersion < localVersion 出错了,请检查！！！");
        }
    }

    public IEnumerator StartUpdate(VersionRes server, VersionRes local)
    {
        //对比本地和服务器的MD5值，找出需要更新的文件
        List<VersionResData> updateList = new List<VersionResData>();
        for(int i = 0; i < server.resList.Count; i++)
        {
            string resName = server.resList[i].resName;
            VersionResData resData = local.resList.Find((item)=> { return item.resName == resName; });
            if(resData != null)
            {
                if(resData.resMD5 != server.resList[i].resMD5)
                {
                    updateList.Add(server.resList[i]);
                }
            }
        }

        //开始加载资源
        for(int i = 0; i < updateList.Count; i++)
        {
            //首先要判断这个文件是否存在，并且MD5码是否匹配，来判断是否下载这个文件。（因为这个文件可能没写入完成，用户就退出了，有这种情况）
            string filePath = string.Format("{0}{1}/{2}", PathManager.RES_PERSISTENT_ROOT_PATH, PathManager.GetPlatform(), updateList[i].resName);
            bool isLoad = false;
            if(File.Exists(filePath))
            {
                string md5 = FileUtility.MD5File(filePath);
                if(md5 != updateList[i].resMD5)
                {
                    isLoad = true;
                    File.Delete(filePath);
                }
            }
            else
            {
                isLoad = true;
            }

            if(isLoad)
            {
                WWW www = new WWW(PathManager.GetServerURL(updateList[i].resName));
                yield return www;
                string directoryPath = Path.GetDirectoryName(filePath);
                FileUtility.CreateDirectory(directoryPath);
                FileStream fs = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(www.bytes);
                bw.Close();
                fs.Close();
                fs.Dispose();
            }
        }

        string str = JsonUtility.ToJson(server);
        FileStream verionFS = File.Open(PathManager.RES_PERSISTENT_ROOT_PATH + PathManager.GetPlatform() + "/VersionRes.txt", FileMode.Create, FileAccess.ReadWrite);
        StreamWriter sw = new StreamWriter(verionFS);
        sw.Write(str);
        sw.Close();
        verionFS.Close();
        verionFS.Dispose();
    }
}
