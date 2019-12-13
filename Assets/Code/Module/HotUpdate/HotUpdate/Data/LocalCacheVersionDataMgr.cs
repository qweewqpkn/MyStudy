using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.HotUpdate
{
    /// <summary>
    /// 本地缓存版本文件管理器
    /// </summary>
    public class LocalCacheVersionDataMgr
    {
        private BaseVersionData _verData = new BaseVersionData();
        private HashSet<string> _cacheFileNames = new HashSet<string>();

        public BaseVersionData VersionData { get { return _verData; } }

        public void AddCacheFile(string fileName)
        {
            _cacheFileNames.Add(fileName.ToLower());
        }

        public void Init(string jsonStr)
        {
            LocalCacheVersionData cache = JsonUtility.FromJson<LocalCacheVersionData>(jsonStr);
            _verData.Version = cache.version;
            _cacheFileNames.Clear();
            for (int i = 0; i < cache.fileNames.Length; i++)
            {
                _cacheFileNames.Add(cache.fileNames[i]);
            }
        }

        public bool HasCache(string fileName)
        {
            return _cacheFileNames.Contains(fileName.ToLower());
        }

        public void Clear()
        {
            _verData.Clear();
        }

        public string ToJson()
        {
            LocalCacheVersionData cache = new LocalCacheVersionData();
            cache.version = _verData.Version;
            cache.fileNames = _cacheFileNames.ToArray();

            return JsonUtility.ToJson(cache);
        }
    }
}
