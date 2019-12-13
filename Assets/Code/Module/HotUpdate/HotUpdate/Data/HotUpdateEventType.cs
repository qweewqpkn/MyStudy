using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.HotUpdate
{
    public enum HotUpdateEventType
    {
        RequestRemoteVersionFail=1,             // 获取服务器Version.manifest失败
        RequestRemoteVersionSuccess,
        RequestRemoteProjectManifestFail,       // 获取服务器Project.manifest失败
        RequestRemoteProjectManifestSuccess,
        NeedPackageUpdate,                      // 需要更新整包
        StartHotUpdateNotify,                   // 开始热更新提醒
        HotUpdateDownloadProgress,              // 热更新下载进度
        HotUpdateDownloadFail,                  // 热更新下载失败
        HotUpdateAllDownloadSuccess,            // 热更新下载完毕
        StartUnzipNotify,                       // 开始解压下载的zip文件
        DiskSpaceInsufficient,                  // 磁盘空间不足
        UnzipProgress,                          // zip解压进度
        UnzipFail,                              // zip解压失败
        UnzipSuccess,                           // zip解压成功
        HotUpdateComplete,                      // 热更新结束

        UnzipFileNotify,                        //zip解压文件通知

        Max = 100,
    }
}
