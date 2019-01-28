using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAB
{
    [MenuItem("Tools/AssetBundle/打包资源")]
    public static void Build()
    {
        string path = "";
        switch(EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.StandaloneWindows64:
                {
                    path = PathManager.RES_PATH_WINDOWS + "/Assetbundle";
                }
                break;
            case BuildTarget.Android:
                {
                    path = PathManager.RES_PATH_ANDROID + "/Assetbundle";
                }
                break;
            case BuildTarget.iOS:
            case BuildTarget.StandaloneOSX:   
                {
                    path = PathManager.RES_PATH_IOS + "/Assetbundle";
                }
                break;
        }
        FileUtility.CreateDirectory(path);
        RemoveUnUseAB(path);
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        Debug.Log("打包完成");
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
            string name = Path.GetFileNameWithoutExtension(buildABList[i]);
            bool isHave = ArrayUtility.Contains(curABNameList, name);
            if(!isHave)
            {
                File.Delete(buildABList[i]);
                Debug.Log("delete unuse ab : " + name);
            }
        }
    }

    [MenuItem("Tools/AssetBundle/拷贝AB到StreamingAssets")]
    static void CopyAB2StreamingAssets()
    {
        string sourceDir = Application.dataPath + "/../ClientRes";
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.iOS:
                {
                    sourceDir += "/IOS";
                }
                break;
            case BuildTarget.Android:
                {
                    sourceDir += "/Android";
                }
                break;
        }
        string targetDir = Application.dataPath + "/StreamingAssets";
        FileUtility.CopyTo(sourceDir, targetDir);
    }

    [MenuItem("Tools/AssetBundle/设置AB Name")]
    static void SetABName()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        AssetImporter ai = AssetImporter.GetAtPath(assetPath);
        int index = assetPath.LastIndexOf('/');     
        ai.assetBundleName = assetPath.Substring(index + 1).Split('.')[0];
    }

    [MenuItem("Tools/AssetBundle/设置Shader Name")]
    static void SetShaderName()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        AssetImporter ai = AssetImporter.GetAtPath(assetPath);
        ai.assetBundleName = "allshader";
    }

    [MenuItem("Tools/AssetBundle/重置所有Shader Name")]
    static void ResetAllABName()
    {
        string[] assetsPath = AssetDatabase.GetAllAssetPaths();

        for(int i = 0; i < assetsPath.Length; i++)
        {
            AssetImporter ai = AssetImporter.GetAtPath(assetsPath[i]);
            if(ai != null)
            {
                ai.assetBundleName = "";
            }
        }
    }
}
