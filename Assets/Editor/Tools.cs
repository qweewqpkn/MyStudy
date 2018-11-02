using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Tools : MonoBehaviour {

	[MenuItem("打包/设置资源AB")]
    public static void SetABName()
    {
        Object[] objects = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
        for(int i = 0; i < objects.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(objects[i]);
            AssetImporter ai = AssetImporter.GetAtPath(path);
            ai.assetBundleName = Path.GetFileNameWithoutExtension(path);
        }
    }

    [MenuItem("打包/打包资源")]
    public static void PackageAB()
    {
        string path = Application.dataPath + "/../ClientRes/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }
}
