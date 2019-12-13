using System;
using Framework.Download;

namespace Framework.HotUpdate
{
    /// <summary>
    /// 服务器返回的版本更新信息模板类，参考：
    /// http://wiki.info/pages/viewpage.action?pageId=4128794
    /// </summary>
    [Serializable]
    public class ResponseProjectManifestData
    {
        public string appUrl;               // 第三方软件更新包地址
        public DownloadFileInfo[] assets;  // 热更新包文件名称数组
        //public long uncompressedSize;       // 补丁包解压后的大小
        public string packageUrl;           // 热更新包的根路径，热更新包文件都是相对于这个路径
        public string packageUrl_backup;    // packageUrl对应的回源url
        public string remoteManifestUrl;    // version.manifest url，可用于域名切换等
        public string remoteVersionUrl;     // project.manifest url，可用于域名切换等
        public string[] searchPaths;        // 目前只有造物用到
        public string version;              // 版本号
        public string versionName;
    }
}
