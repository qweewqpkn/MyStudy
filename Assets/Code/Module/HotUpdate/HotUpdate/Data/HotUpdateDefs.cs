using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

namespace Framework.HotUpdate
{
    public static class HotUpdateDefs
    {
        public static readonly string kBreakpointTransferInfoFile = "hotupdate_breakpoint_transfer_info.cfg";// MD5Hash.Get("hotupdate_breakpoint_transfer_info.cfg");
        public static readonly string kPackageVersionFile = "version";// 包版本号文件version.manifest，存于Resources目录
        // 热更文件列表信息
        public static readonly string kLocalCacheVersionDataFile = "localcache_versiondata";// MD5Hash.Get("localcache_versiondata");
    }
}
