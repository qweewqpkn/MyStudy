
using AssetLoad;
using System.Text;
using UnityEngine;

public class PathManager
{
    //本地资源根路径
    public static string RES_LOCAL_ROOT_PATH = Application.dataPath + "/../ClientRes";
    public static string RES_STREAM_ROOT_PATH = Application.streamingAssetsPath + "/ClientRes";
    public static string RES_PERSISTENT_ROOT_PATH = Application.persistentDataPath + "/ClientRes";

    //服务器资源路径
    public static string RES_SERVER_ROOT_PATH = "";

    public static string GetServerURL(string name)
    {
        StringBuilder result = new StringBuilder();
        result.Append("file://");
        result.Append(RES_SERVER_ROOT_PATH);
        result.Append(GetPlatform());
        result.Append("/");
        result.Append(name);
        return result.ToString();
    }

    public static string GetPlatform()
    {
        StringBuilder result = new StringBuilder();
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                {
                    result.Append("/Android");
                }
                break;
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                {
                    result.Append("/IOS");
                }
                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                {
                    result.Append("/Windows");
                }
                break;
        }

        return result.ToString();
    }

    public static string URL(string abName, AssetType type, bool isWWW = true)
    {
        StringBuilder result = new StringBuilder();
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                {
                    result.Append(RES_STREAM_ROOT_PATH);
                    result.Append("/Android");
                }
                break;
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.OSXPlayer:
                {
                    if(isWWW)
                    {
                        result.Append("file://");
                    }
                    result.Append(RES_STREAM_ROOT_PATH);
                    result.Append("/IOS");
                }
                break;
            case RuntimePlatform.OSXEditor:
                {
                    if (isWWW)
                    {
                        result.Append("file://");
                    }
                    result.Append(RES_LOCAL_ROOT_PATH);
                    result.Append("/IOS");
                }
                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                {
                    if (isWWW)
                    {
                        result.Append("file://");
                    }
                    result.Append(RES_LOCAL_ROOT_PATH);
                    result.Append("/Windows");
                }
                break;
        }

        switch (type)
        {
            case AssetType.eAB:
            case AssetType.eAudioClip:
            case AssetType.eManifest:
            case AssetType.ePrefab:
            case AssetType.eShader:
            case AssetType.eSprite:
            case AssetType.eTexture:
                {
                    result.Append("/Assetbundle");
                }
                break;
            case AssetType.eText:
                {
                    result.Append("/Config");
                }
                break;
            case AssetType.eLua:
                {
                    result.Append("/Lua");
                }
                break;
        }

        result.Append("/");
        result.Append(abName);
        return result.ToString();
    }
}

