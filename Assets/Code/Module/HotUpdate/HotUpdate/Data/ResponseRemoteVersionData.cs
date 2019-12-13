using System;

namespace Framework.HotUpdate
{
    /// <summary>
    /// 服务返回的version.manifest模板类
    /// {"version":"1.0.0"}
    /// 参考：
    /// http://wiki.info/pages/viewpage.action?pageId=4128794
    /// </summary>
    public class ResponseVersionManifestData
    {
        public string version;
        public int zoneId;
        public bool review;
        public string loginUrl;
        public int force;

        public ResponseVersionManifestData()
        {
            version = string.Empty;
            zoneId = 0;
            review = false;
            loginUrl = string.Empty;
            force = 0;
        }
    }
}
