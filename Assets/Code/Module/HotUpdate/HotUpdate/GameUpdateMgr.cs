/*-------------------------------------------------------------------
// Copyright (C)
//
// Module: GameUpdateMgr
// Author: huangxin
// Date: 2017.09.04
// Description: Game update management.
//-----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Framework.Download;

namespace Framework.HotUpdate
{
    public class GameUpdateMgr : Singleton<GameUpdateMgr>
    {
        private readonly float kDownloadProgressTickInterval = 0.03333f;

        private DownloadMgr _downloadMgr;

        private UnzipThread _unzipThread;

        private string _versionManifestUrl;
        private string _projectManifestUrl;
        private bool _isDownloadFromSourceResServer = false; // 是否直接从源服务器热更资源

        private int _currentTaskIndex; // 当前正在下载的任务索引
        private List<string> _updateList; // 下载任务列表
        //private List<string> _completeList = new List<string>(); // 下载完成的任务列表

        //// 获取服务器版本信息失败通知事件，param：重新获取回调
        //public Action<Action> RequestRemoteVersionFailedHandler;

        //// 获取版本更新信息失败通知事件，param：重新获取回调
        //public Action<Action> RequestRemoteProjectManifestFailedHandler;

        //// 开始下载通知事件，param1: 已接收字节大小，param2: 需要下载的总文件字节大小，param3: 下载回调
        //public Action<long, long, Action> StartDownloadNotifyHandler;
        //// 下载进度通知事件，param1: 已接收字节大小，param2: 需要下载的总文件字节大小
        //public Action<long, long> DownloadProgressHandler;
        //// 下载文件失败通知事件，param1: 重新下载，param2： 错误类型
        //public Action<Action, DownloadError> DownloadFileFailedHandler;
        //// 更新包下载完成回调
        //public Action DownloadCompleteHandler;
        //// 解压更新包进度通知事件
        //public Action<float> UnzipProgressHandler;
        //// 解压失败通知事件，param1: 重试，param2: 错误类型
        //public Action<Action, UnzipStatus> UnzipFailedHandler;

        //// 更新完毕回调
        //public Action UpdateCompletedHandler;

        private long startUpdateTime = 0;
        private long endUpdateTime = 0;
        /// <summary>
        /// 获取更新时间
        /// </summary>
        /// <returns></returns>
        public long GetUpdateTime()
        {
            return endUpdateTime - startUpdateTime;
        }

        private long totalSize = 0;
        /// <summary>
        /// 获取热更总大小
        /// </summary>
        /// <returns></returns>
        public long GetTotalSize()
        {
            return totalSize;
        }


        public GameUpdateMgr()
        {
            _downloadMgr = new DownloadMgr(FileUtils.LocalTempResRootPath, HotUpdateDefs.kBreakpointTransferInfoFile);
        }

        /// <summary>
        /// CDN故障时强制回源
        /// </summary>
        public void SetDownloadFromSourceResServer()
        {
            _isDownloadFromSourceResServer = true;
        }

        /// <summary>
        /// 请求远程服务器最新的版本号
        /// </summary>
        /// <param name="failedCb"></param>
        /// <param name="finishCb">传入最新的版本信息ResponseVersionManifestData</param>
        public void RequestRemoteVersionCode(Action<string> failedCb, Action<ResponseVersionManifestData> finishCb)
        {
            MainThreadRunner.Instance.StartCoroutine(VersionMgr.Instance.RequestRemoteVersionCode(_versionManifestUrl, failedCb, finishCb));
        }

        /// <summary>
        /// 请求远程服务器最新的版本信息
        /// </summary>
        /// <param name="failedCb"></param>
        /// <param name="finishCb">传入最新的版本信息ResponseVersionManifestData</param>
        public void RequestRemoteVersion(Action<string> failedCb, Action<ResponseVersionManifestData> finishCb,
            string domain, string appCode, int platform, string channel, int versionCode, string packageName)
        {
            if (VersionMgr.Instance.LoadLocalVersionCode()) 
            {
                string protocolDomain = GetProtocolDomain(domain);
                string versionManifestUrl = string.Format("{0}/manifest/{1}/version_v3.manifest?p={2}&c1={3}&v={4}&vc={5}&pn={6}",
                        protocolDomain, appCode, platform, channel, VersionMgr.Instance.localVersion, versionCode, packageName);
                MainThreadRunner.Instance.StartCoroutine(VersionMgr.Instance.RequestRemoteVersionCode(versionManifestUrl, failedCb, finishCb));
            }
            else
            {
                failedCb("VersionMgr::LoadLocalVersionCode(), LocalVersionCode is null!");
            }
        }

        /// <summary>
        /// 获取带http或https的domain
        /// </summary>
        /// <param name="domain">原始domain</param>
        /// <returns>domain</returns>
        private string GetProtocolDomain(string domain)
        {
            string protocolDomain = domain;
            if (!string.IsNullOrEmpty(domain) && !domain.Contains("http://") && !domain.Contains("https://"))
            {
                protocolDomain = string.Format("http://{0}", domain);
            }
            return protocolDomain;
        }

        /// <summary>
        /// 开始更新 - v3
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="appCode">游戏代码</param>
        /// <param name="platform">是平台，（0=Android，1=IOS，2=IOS测试，3=海外，4=PC）</param>
        /// <param name="channel">渠道配置的渠道id</param>
        /// <param name="versionCode">客户端版本号</param>
        /// <param name="packageName">包名</param>
        public void CheckUpdate(string domain, string appCode, int platform, string channel, int versionCode, string packageName)
        {
            if (VersionMgr.Instance.LoadLocalVersionCode())
            {
                string protocolDomain = GetProtocolDomain(domain);
                string versionManifestUrl = string.Format("{0}/manifest/{1}/version_v3.manifest?p={2}&c1={3}&v={4}&vc={5}&pn={6}",
                    protocolDomain, appCode, platform, channel, VersionMgr.Instance.localVersion, versionCode, packageName);
                string projectManifestUrl = string.Format("{0}/manifest/{1}/project_v3.manifest?p={2}&c1={3}&v={4}&vc={5}&pn={6}",
                    protocolDomain, appCode, platform, channel, VersionMgr.Instance.localVersion, versionCode, packageName);
                CheckUpdate(versionManifestUrl, projectManifestUrl);
            }
        }

        /// <summary>
        /// 开始更新 - v2
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="appCode">游戏代码</param>
        /// <param name="platform">是平台，（0=Android，1=IOS，2=IOS测试，3=海外，4=PC）</param>
        /// <param name="channel">渠道配置的渠道id</param>
        public void CheckUpdate(string domain, string appCode, int platform, string channel)
        {
            if (VersionMgr.Instance.LoadLocalVersionCode())
            {
                string protocolDomain = GetProtocolDomain(domain);
                string versionManifestUrl = string.Format("{0}/manifest/{1}/version.manifest?p={2}&c1={3}&v={4}",
                    protocolDomain, appCode, platform, channel, VersionMgr.Instance.localVersion);
                string projectManifestUrl = string.Format("{0}/manifest/{1}/project_v2.manifest?p={2}&c1={3}&v={4}",
                    protocolDomain, appCode, platform, channel, VersionMgr.Instance.localVersion);
                CheckUpdate(versionManifestUrl, projectManifestUrl);
            }
        }

        /// <summary>
        /// 更新检查
        /// </summary>
        /// <param name="versionManifestUrl">服务器version.manifest的url</param>
        /// <param name="projectManifestUrl">服务器project.manifest的url</param>
        public void CheckUpdate(string versionManifestUrl, string projectManifestUrl)
        {
            if (VersionMgr.Instance.LoadLocalVersionCode())
            {
                _isDownloadFromSourceResServer = false;
                _versionManifestUrl = versionManifestUrl;
                _projectManifestUrl = projectManifestUrl;

                MainThreadRunner.Instance.StartCoroutine(VersionMgr.Instance.LoadRemoteVersionCode(versionManifestUrl, 
                    (string errorInfo) =>
                    {
                        //if (RequestRemoteVersionFailedHandler != null)
                        //    RequestRemoteVersionFailedHandler(() => { CheckUpdate(versionManifestUrl, projectManifestUrl); });
                        EventCenter.Broadcast<string, Action>((int)HotUpdateEventType.RequestRemoteVersionFail, errorInfo, delegate { CheckUpdate(versionManifestUrl, projectManifestUrl); });
                    },
                    (ResponseVersionManifestData manifestData) =>
                    {
                        EventCenter.Broadcast((int)HotUpdateEventType.RequestRemoteVersionSuccess, manifestData);

                        // 比对localVersion与remoteVersion
                        if (VersionMgr.Instance.IsClientNew)
                        {
                            CLogger.Log("GameUpdateMgr::CheckUpdate() - 已经是最新版本，不需要更新.");
                            OnUpdateComplete(); // 版本相同，更新完毕
                        }
                        else
                            UpdateVersion();
                    }));
            }
        }

        private void UpdateVersion()
        {
            if (_downloadMgr == null)
                _downloadMgr = new DownloadMgr(FileUtils.LocalTempResRootPath, HotUpdateDefs.kBreakpointTransferInfoFile);
            MainThreadRunner.Instance.StartCoroutine(VersionMgr.Instance.RequestVersionUpdateInfo(_projectManifestUrl,
                (string errorInfo) =>
                {
                    //if (RequestRemoteProjectManifestFailedHandler != null)
                    //    RequestRemoteProjectManifestFailedHandler(() => { UpdateVersion(); });
                    EventCenter.Broadcast<string, Action>((int)HotUpdateEventType.RequestRemoteProjectManifestFail, errorInfo, delegate { UpdateVersion(); });
                },
                () =>
                {
                    EventCenter.Broadcast((int)HotUpdateEventType.RequestRemoteProjectManifestSuccess);

                    // 检查是否需要更新整包
                    if (!VersionMgr.Instance.IsPackageNew)
                    {
                        // 下载最新安装包
                        UpdatePackage();
                    }
                    // 检查是否需要热更（本地主版本号不可能比服务器主版本号大）
                    else if (VersionMgr.Instance.IsNeedHotUpdate)
                    {
                        // 热更资源
                        UpdateResource();
                    }
                    else
                    {
                        CLogger.Log("GameUpdateMgr::UpdateVersion() - 已经是最新版本，不需要更新.");
                        OnUpdateComplete(); // 版本相同，更新完毕
                    }
                }));
        }

        /// <summary>
        /// 更新整包
        /// </summary>
        private void UpdatePackage()
        {
            CLogger.Log("GameUpdateMgr::UpdatePackage() - Update latest package, url: " + VersionMgr.Instance.projectManifestInfo.appUrl);
            EventCenter.Broadcast((int)HotUpdateEventType.NeedPackageUpdate, VersionMgr.Instance.projectManifestInfo.appUrl);
            Dispose();
        }

        /// <summary>
        /// 更新资源
        /// </summary>
        private void UpdateResource()
        {
            startUpdateTime = Utility.GetCurrentUTC() / 1000;
            _updateList = _downloadMgr.StartContinueTransfer(new List<DownloadFileInfo>(VersionMgr.Instance.projectManifestInfo.assets));
            if (_updateList.Count > 0)
            {
                CLogger.Log("GameUpdateMgr::UpdateResource() - Start hot update resource.");
                _currentTaskIndex = 0;
                long leftDownloadBytes = _downloadMgr.TotalBytesToReceive - _downloadMgr.TotalReceivedBytes;
                if (leftDownloadBytes > 0/* && StartDownloadNotifyHandler != null*/)
                {
                    long diskLeftSpace = NativeManager.instance.GetFreeDiskSpace();//NativeMgr.Instance.CallStaticLongDefault("com.baitian.wrap.StorageWrap", "getAvailableInternalSize", "{}", -1);
                    Debug.Log("###########diskLeftSpace=" + diskLeftSpace);
                    if (diskLeftSpace != -1)
                    {
                        if (diskLeftSpace < (_downloadMgr.TotalBytesToReceive/*VersionMgr.Instance.projectManifestInfo.uncompressedSize*/ - _downloadMgr.TotalReceivedBytes + 50 * 1024 * 1024L))
                        {
                            CLogger.LogWarn("GameUpdateMgr::UpdateResource() - 磁盘空间必须至少是补丁包解压后大小+50MB, diskLeftSpace:" + diskLeftSpace + ", 补丁包大小:" + leftDownloadBytes);
                            // 磁盘空间不足
                            EventCenter.Broadcast((int)HotUpdateEventType.DiskSpaceInsufficient);
                        }
                        else
                        {
                            EventCenter.Broadcast<long, long, Action>((int)HotUpdateEventType.StartHotUpdateNotify, _downloadMgr.TotalReceivedBytes,
                                _downloadMgr.TotalBytesToReceive, delegate { DownloadUpdateList(); });
                        }
                    }
                    else // 返回-1表示本地代码还未提供获取磁盘剩余空间的API实现
                    {
                        //StartDownloadNotifyHandler(_downloadMgr.transferMgr.TotalReceivedBytes, _downloadMgr.transferMgr.TotalBytesToReceive, () =>
                        //    {
                        //        DownloadUpdateList();
                        //    });
                        EventCenter.Broadcast<long, long, Action>((int)HotUpdateEventType.StartHotUpdateNotify, _downloadMgr.TotalReceivedBytes,
                            _downloadMgr.TotalBytesToReceive, delegate { DownloadUpdateList(); });
                    }
                }
                else
                    DownloadUpdateList();
            }
            else
            {
                CLogger.Log("GameUpdateMgr::UpdateResource() - No need hot update.");
                OnUpdateComplete();
            }
        }

        /// <summary>
        /// 下载热更文件
        /// </summary>
		private void DownloadUpdateList()
        {
            totalSize = _downloadMgr.TotalReceivedBytes;

            _downloadMgr.StartService();
            StartDownloadProgressTick();
            _downloadMgr.AsyncDownloadList(_updateList, (!_isDownloadFromSourceResServer) ? 
                VersionMgr.Instance.projectManifestInfo.packageUrl : VersionMgr.Instance.projectManifestInfo.packageUrl_backup,
                FileUtils.LocalTempResRootPath, OnFileDownload);
            
        }

        private void OnFileDownload(DownloadTask task)
        {
            if (task.status == DownloadStatus.Complete)
            {
                //_completeList.Add(task.storagePath);
                _currentTaskIndex++;
                if (_currentTaskIndex == _updateList.Count)
                    OnAllDownloaded();
            }
            else
            {
                string log = "GameUpdateMgr::OnFileDownload() - Failed:" + task.url + "#" + _currentTaskIndex + "," + task.errorCode.ToString();
                CLogger.LogWarn(log);
                _downloadMgr.StopService();
                //失败了也需要保存当前已更新的版本数据
                _downloadMgr.SaveTransferProgress("SaveOnError:" + task.file);
                //if (DownloadFileFailedHandler != null)
                //    DownloadFileFailedHandler(() => { DownloadMgr.Instance.StartService(); }, task.errorCode);

                EventCenter.Broadcast<DownloadTask, string, Action>((int)HotUpdateEventType.HotUpdateDownloadFail, task, log, delegate {
                    //_downloadMgr.Dispose();
                    //_downloadMgr = new DownloadMgr(FileUtils.LocalTempResRootPath, HotUpdateDefs.kBreakpointTransferInfoFile);
                    //DownloadUpdateList();

                    _downloadMgr.StartService();
                    _downloadMgr.AsyncDownloadList(_updateList, (!_isDownloadFromSourceResServer) ?
                        VersionMgr.Instance.projectManifestInfo.packageUrl : VersionMgr.Instance.projectManifestInfo.packageUrl_backup,
                        FileUtils.LocalTempResRootPath, OnFileDownload);

                    ; });
            }
        }

        private void OnAllDownloaded()
        {
            _updateList.Clear();
            //if (DownloadCompleteHandler != null)
            //    DownloadCompleteHandler();
            EventCenter.Broadcast((int)HotUpdateEventType.HotUpdateAllDownloadSuccess);
            _downloadMgr.StopService();
            _downloadMgr.SaveTransferProgress("SaveOnAllDownloaded:");
            CloseDownloadProgressTick();

            _unzipThread = new UnzipThread(_downloadMgr.AllTransferFiles);
            _unzipThread.StartUnzip(OnUnzipFinished/*, UnzipProgressHandler*/);
        }

        public void OnUnzipProgress(long bytesTransferred, long totalBytesToTransfer)
        {
            _unzipThread.SetTotalUnzipProgress(bytesTransferred, totalBytesToTransfer);
        }

        private void OnUnzipFinished(UnzipStatus status)
        {
            if (VersionMgr.Instance.remoteVerCode.Version != VersionMgr.Instance.localCacheVerCode.Version
                /*&& UnzipFailedHandler != null*/)
            {
                Debug.Log("VersionMgr.Instance.remoteVerCode.Version:" + VersionMgr.Instance.remoteVerCode.Version);
                Debug.Log("VersionMgr.Instance.localCacheVerCode.Version:" + VersionMgr.Instance.localCacheVerCode.Version);
                //UnzipFailedHandler(() => { _downloadMgr.transferMgr.DownloadFinished(OnHandlerFinished, UnzipProgressHandler); }, status);
                EventCenter.Broadcast<UnzipStatus, Action>((int)HotUpdateEventType.UnzipFail, status, delegate { _unzipThread.StartUnzip(OnUnzipFinished); });
            }
            else
            {
                EventCenter.Broadcast((int)HotUpdateEventType.UnzipSuccess);
                //OnUpdateComplete();

                endUpdateTime = Utility.GetCurrentUTC() / 1000;

                // 清理即可，不再调用Lua因为马上要执行重启Lua
                Dispose();

                CLogger.Log("GameUpdateMgr::OnUnzipFinished()!");

                // 释放热更界面
                ClearUI();
                // 加载新的资源缓存
                LocalCacheFilesMgr.Instance.LoadCacheFiles();
                // 清理事件
                EventCenter.Cleanup();
                GameUpdateHelper.Instance.ClearListeners();
                // 释放资源
                AssetLoad.ResourceManager.instance.ReleaseAll();
                // 调试，面板也清理一下
                Debuger.DisposeOnRestart();
                // 重启xlua
                LuaManager.instance.ReStart();
            }
        }

        private void ClearUI()
        {
            //如果不删除热更界面，在Android上回黑屏？？？
            GameObject ui_root = GameObject.Find("UIRoot");
            if (ui_root != null)
            {
                Transform ui2 = ui_root.transform.Find("ui_hot_update(Clone)");
                if (ui2 != null)
                {
                    Debug.Log("#################DestroyImmediate ui_hot_update");
                    GameObject.DestroyImmediate(ui2.gameObject);
                }
            }
        }

        private void StartDownloadProgressTick()
        {
            //_completeList.Clear();
            Scheduler.Instance.AddListener(DownloadProgressTick, kDownloadProgressTickInterval);
        }

        private void CloseDownloadProgressTick()
        {
            Scheduler.Instance.RemoveListener(DownloadProgressTick);
        }

        private void DownloadProgressTick()
        {
            //if (DownloadProgressHandler != null)
            //    DownloadProgressHandler(_downloadMgr.transferMgr.TotalReceivedBytes, _downloadMgr.transferMgr.TotalBytesToReceive);
            EventCenter.Broadcast<long, long, string>((int)HotUpdateEventType.HotUpdateDownloadProgress, _downloadMgr.TotalReceivedBytes,
                _downloadMgr.TotalBytesToReceive, _downloadMgr.CurrentTaskFileName);
        }

        private void OnUpdateComplete()
        {
            //if (UpdateCompletedHandler != null)
            //    UpdateCompletedHandler();
            EventCenter.Broadcast((int)HotUpdateEventType.HotUpdateComplete);

            // 清理
            Dispose();
        }

        /// <summary>
        /// 资源清理
        /// </summary>
        private void Dispose()
        {
            if (_updateList != null)
                _updateList.Clear();
            //if (_completeList != null)
            //    _completeList.Clear();
            if(_downloadMgr != null)
                _downloadMgr.Dispose();
            _downloadMgr = null;
            _unzipThread = null;
        }
    }
}