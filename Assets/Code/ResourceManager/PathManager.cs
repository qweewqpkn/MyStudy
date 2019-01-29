
using AssetLoad;
using System.Text;
using UnityEngine;

public class PathManager
{
    public static string RES_ROOT_PATH = Application.dataPath + "/../ClientRes";

    public static string RES_PATH_ANDROID = RES_ROOT_PATH + "/Android";
    public static string RES_PATH_ANDROID_PHONE = Application.streamingAssetsPath + "/ClientRes/Android";
    public static string RES_PATH_ANDRIOD_PHONE_UPDATE = Application.persistentDataPath + "/ClientRes/Android";

    public static string RES_PATH_IOS = RES_ROOT_PATH + "/IOS";
    public static string RES_PATH_IOS_PHONE = Application.streamingAssetsPath + "/ClientRes/IOS";
    public static string RES_PATH_IOS_PHONE_UPDATE = Application.persistentDataPath + "/ClientRes/IOS";

    public static string RES_PATH_WINDOWS = RES_ROOT_PATH + "/Windows";

    public static string URL(string abName, AssetType type)
    {
        StringBuilder result = new StringBuilder();
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                {
                    result.Append(RES_PATH_ANDROID_PHONE);
                }
                break;
            case RuntimePlatform.IPhonePlayer:
                {
                    result.Append("file://" + RES_PATH_IOS_PHONE);
                }
                break;
            case RuntimePlatform.OSXEditor:
                {
                    result.Append("file://" + RES_PATH_IOS);
                }
                break;
            case RuntimePlatform.WindowsEditor:
                {
                    result.Append("file://" + RES_PATH_WINDOWS);
                }
                break;
            default:
                {
                    result.Append("file://" + RES_PATH_WINDOWS);
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
                    result.Append("/Assetbundle/");
                }
                break;
            case AssetType.eText:
                {
                    result.Append("/Config/");
                }
                break;
        }

        result.Append(abName);
        return result.ToString();
    }
}

