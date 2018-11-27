using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAB
{
    [MenuItem("Tools/AssetBundle/打包资源")]
    static void Build()
    {
        string path = Application.dataPath +  "/../ClientRes/";
        switch(EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.StandaloneWindows64:
                {
                    path += "Windows";
                }
                break;
            case BuildTarget.Android:
                {
                    path += "Android";
                }
                break;
        }
        path += "/Assetbundle/";

        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

        Debug.Log("打包完成");
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
