
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
}

