using System;

namespace Framework.HotUpdate
{
    /// <summary>
    /// 缓存从服务器热更下来的ab文件信息，
    /// 从服务器热更下来的ab文件都存放在PersistentPath目录下，
    /// 而随包的ab都存放在StreamingAssetsPath目录下，
    /// 加载ab的时候需要注意ab的存放目录
    /// </summary>
    [Serializable]
    public class LocalCacheVersionData
    {
        public string version;
        public string[] fileNames;
    }
}

