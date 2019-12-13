using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.HotUpdate
{
    public class LocalCacheFilesMgr : Singleton <LocalCacheFilesMgr>
    {
        private HashSet<string> _cacheFileNames = new HashSet<string>();

        private bool loaded = false;

        public void LoadCacheFiles()
        {
            string localVerFilePath = Application.persistentDataPath + "/ClientRes/" + PathManager.GetRuntimePlatform() + "/" + HotUpdateDefs.kLocalCacheVersionDataFile;
            if (File.Exists(localVerFilePath))
            {
                string jsonStr = File.ReadAllText(localVerFilePath);
                LocalCacheVersionData cache = JsonUtility.FromJson<LocalCacheVersionData>(jsonStr);
                _cacheFileNames.Clear();
                for (int i = 0; i < cache.fileNames.Length; i++)
                {
                    _cacheFileNames.Add(cache.fileNames[i]);
                }
            }
        }

        public bool HasCache(string fileName, AssetLoad.AssetType type)
        {
            if (!loaded)
            {
                LoadCacheFiles();
                loaded = true;
            }
            string resType = "";
            //特定资源
            switch (type)
            {
                default:
                    {
                        resType = "assetbundle/";
                    }
                    break;
                case AssetLoad.AssetType.eLua:
                    {
                        resType = "lua/";
                    }
                    break;
                case AssetLoad.AssetType.eVideo:
                    {
                        resType = "video/";
                    }
                    break;
            }
            return _cacheFileNames.Contains(resType + fileName.ToLower());
        }
    }
}
