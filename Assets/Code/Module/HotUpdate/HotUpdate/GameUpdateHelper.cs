using XLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework;
using Framework.Download;

namespace Framework.HotUpdate
{
    public class LuaHandlerEntry
    {
        public LuaFunction _handler;
        public LuaTable _handlerTarget;
        public LuaHandlerEntry(LuaFunction handler, LuaTable handlerTarget)
        {
            _handler = handler;
            _handlerTarget = handlerTarget;
        }


        public void Call()
        {
            _handler.Call(_handlerTarget);
        }
        public void Call(string args)
        {
            _handler.Call(_handlerTarget, args);
        }
        public void Call(string[] args)
        {
            _handler.Call(_handlerTarget, args);
        }
        public void Call(int args)
        {
            _handler.Call(_handlerTarget, args);
        }
        public void Call(float args)
        {
            _handler.Call(_handlerTarget, args);
        }
        public void Call(long args1, long args2)
        {
            _handler.Call(_handlerTarget, args1, args2);
        }
        public void Call(long args1, long args2, string args3)
        {
            _handler.Call(_handlerTarget, args1, args2, args3);
        }
        public void Call(string args1, int args2, bool args3, string args4, int args5)
        {
            _handler.Call(_handlerTarget, args1, args2, args3, args4, args5);
        }
        public void Call(string args1, int args2, string args3, long args4, string args5)
        {
            _handler.Call(_handlerTarget, args1, args2, args3, args4, args5);
        }
    }


    public class GameUpdateHelper
	{
		public static GameUpdateHelper Instance = new GameUpdateHelper();

		private Dictionary<HotUpdateEventType, LuaHandlerEntry> handlerMap = new Dictionary<HotUpdateEventType, LuaHandlerEntry>();
		private Action nextAction;
		
		private GameUpdateHelper()
		{
		}

		/// <summary>
		/// 给某个热更新阶段添加响应事件
		/// </summary>
		/// <param name="eventTypeId">对应lua端的HotUpdateEventType</param>
		/// <param name="handler">事件的响应函数，不同事件的参数不同</param>
		/// <param name="handlerTarget">调用target</param>
		public void AddListener(HotUpdateEventType eventTypeId, LuaFunction handler, LuaTable handlerTarget)
		{
            this.handlerMap.Add(eventTypeId, new LuaHandlerEntry(handler, handlerTarget));
        }

        /// <summary>
        /// 清除所有已注册的事件监听
        /// </summary>
        public void ClearListeners()
        {
            this.handlerMap.Clear();
        }

		/// <summary>
        /// 统一添加热更阶段响应事件监听
        /// </summary>
        private void AddUpdateListeners()
        {
            EventCenter.AddListener<string, System.Action>((int)HotUpdateEventType.RequestRemoteVersionFail, OnRequestRemoteVersionFail);
            EventCenter.AddListener<ResponseVersionManifestData>((int)HotUpdateEventType.RequestRemoteVersionSuccess, OnRequestRemoteVersionSuccess);
            EventCenter.AddListener<string, System.Action>((int)HotUpdateEventType.RequestRemoteProjectManifestFail, OnRequestRemoteProjectManifestFail);
            EventCenter.AddListener((int)HotUpdateEventType.RequestRemoteProjectManifestSuccess, OnRequestRemoteProjectManifestSuccess);
            EventCenter.AddListener<string>((int)HotUpdateEventType.NeedPackageUpdate, OnNeedPackageUpdate);
            EventCenter.AddListener<long, long, System.Action>((int)HotUpdateEventType.StartHotUpdateNotify, OnStartHotUpdateNotify);
            EventCenter.AddListener<long, long, string>((int)HotUpdateEventType.HotUpdateDownloadProgress, OnHotUpdateDownloadProgress);
            EventCenter.AddListener<DownloadTask, string, System.Action>((int)HotUpdateEventType.HotUpdateDownloadFail, OnHotUpdateDownloadFail);
            EventCenter.AddListener((int)HotUpdateEventType.HotUpdateAllDownloadSuccess, OnHotUpdateAllDownloadSuccess);
            EventCenter.AddListener((int)HotUpdateEventType.StartUnzipNotify, OnStartUnzipNotify);
            EventCenter.AddListener<float>((int)HotUpdateEventType.UnzipProgress, OnUnzipProgress);
            EventCenter.AddListener((int)HotUpdateEventType.UnzipSuccess, OnUnzipSuccess);
            EventCenter.AddListener<UnzipStatus, System.Action>((int)HotUpdateEventType.UnzipFail, OnUnzipFail);
            EventCenter.AddListener((int)HotUpdateEventType.DiskSpaceInsufficient, OnDiskSpaceInsufficient);
            EventCenter.AddListener((int)HotUpdateEventType.HotUpdateComplete, OnHotUpdateComplete);
            EventCenter.AddListener<string[]>((int)HotUpdateEventType.UnzipFileNotify, OnUnzipFileNotify);
        }

        /// <summary>
        /// 请求远程服务器最新的版本信息
        /// </summary>
        public void RequestRemoteVersion(string domain, string appCode, int platform, string channel, int versionCode, string packageName)
        {
            GameUpdateMgr.Instance.RequestRemoteVersion(OnRequestRemoteVersionFail, OnRequestRemoteVersionSuccess,
                domain, appCode, platform, channel, versionCode, packageName);
        }

        /// <summary>
        /// 开始热更新 - v3
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="appCode">游戏编码</param>
        /// <param name="platform">平台</param>
        /// <param name="channel">渠道</param>
        /// <param name="versionCode">包版本号</param>
        /// <param name="packageName">包名</param>
        public void StartUpdate(string domain, string appCode, int platform, string channel, int versionCode, string packageName)
        {
            AddUpdateListeners();
            GameUpdateMgr.Instance.CheckUpdate(domain, appCode, platform, channel, versionCode, packageName);
        }

		/// <summary>
        /// 开始热更新 - v2
        /// </summary>
        /// <param name="domain">域名</param>
        /// <param name="appCode">游戏编码</param>
        /// <param name="platform">平台</param>
        /// <param name="channel">渠道</param>
        public void StartUpdate(string domain, string appCode, int platform, string channel)
        {
            AddUpdateListeners();
            GameUpdateMgr.Instance.CheckUpdate(domain, appCode, platform, channel);
        }

        /// <summary>
		/// 开始热更新 - 自定义url
		/// </summary>
		/// <param name="versionManifestUrl">版本信息文件的url</param>
		/// <param name="projectManifestUrl">版本文件内容的url</param>
		public void StartUpdate(string versionManifestUrl, string projectManifestUrl)
		{
            AddUpdateListeners();
            GameUpdateMgr.Instance.CheckUpdate(versionManifestUrl, projectManifestUrl);
		}

		/// <summary>
		/// 继续下一步操作，用于某些事件要提示后才能继续下一步流程。例如开始下载文件和错误重试
		/// </summary>
		/// <returns></returns>
		public void DoNext()
		{
			if (this.nextAction != null)
				this.nextAction();
		}

        /// <summary>
        /// 获取更新时间
        /// </summary>
        /// <returns></returns>
        public long GetUpdateTime()
        {
            return GameUpdateMgr.Instance.GetUpdateTime();
        }
        /// <summary>
        /// 获取热更总大小
        /// </summary>
        /// <returns></returns>
        public long GetTotalSize()
        {
            return GameUpdateMgr.Instance.GetTotalSize();
        }


        /// <summary>
        /// 请求服务器Version.manifest文件成功回调
        /// </summary>
        private void OnRequestRemoteVersionSuccess(ResponseVersionManifestData manifestData)
		{
			var handler = handlerMap[HotUpdateEventType.RequestRemoteVersionSuccess];
			if(handler != null)
			{
                string remoteVersion = manifestData.version;
                int zoneId = manifestData.zoneId;
                bool review = manifestData.review;
                string loginUrl = manifestData.loginUrl;
                int force = manifestData.force;
				handler.Call(remoteVersion, zoneId, review, loginUrl, force);
			}
		}

		/// <summary>
		/// 请求服务器Project.manifest文件成功回调
		/// </summary>
		private void OnRequestRemoteProjectManifestSuccess()
		{
			var handler = handlerMap[HotUpdateEventType.RequestRemoteProjectManifestSuccess];
			if(handler != null)
			{
				handler.Call();
			}
		}

		/// <summary>
		/// 所有热更新资源下载成功回调
		/// </summary>
		private void OnHotUpdateAllDownloadSuccess()
		{
			var handler = handlerMap[HotUpdateEventType.HotUpdateAllDownloadSuccess];
			if(handler != null)
			{
				handler.Call();
			}
		}
		/// <summary>
		/// 解压热更新资源成功回调
		/// </summary>
		private void OnUnzipSuccess()
		{
			var handler = handlerMap[HotUpdateEventType.UnzipSuccess];
			if(handler != null)
			{
				handler.Call();
			}
		}

		/// <summary>
		/// 热更新结束回调
		/// </summary>
		private void OnHotUpdateComplete()
		{
			var handler = handlerMap[HotUpdateEventType.HotUpdateComplete];
			if(handler != null)
			{
				handler.Call();
			}

		}

        /// <summary>
		/// 热更包具体文件通知
		/// </summary>
		private void OnUnzipFileNotify(string[] allUnzipFileNames)
        {
            var handler = handlerMap[HotUpdateEventType.UnzipFileNotify];
            if (handler != null)
            {
                handler.Call(allUnzipFileNames);
            }

        }



        /// <summary>
        /// 更新整包回调
        /// </summary>
        private void OnNeedPackageUpdate(string packageUrl)
		{
			var handler = handlerMap[HotUpdateEventType.NeedPackageUpdate];
			if(handler != null)
			{
				handler.Call(packageUrl);
			}
		}

		/// <summary>
		/// 开始热更新资源回调
		/// </summary>
		/// <param name="currentReceivedBytes">已经下载的字节数</param>
		/// <param name="totalBytesToReceive">热更新补丁包总字节大小</param>
		/// <param name="startDownload">开始下载Action</param>
		private void OnStartHotUpdateNotify(long currentReceivedBytes, long totalBytesToReceive, System.Action startDownload)
		{
			var handler = handlerMap[HotUpdateEventType.StartHotUpdateNotify];
			this.nextAction = startDownload;
			if(handler != null)
			{
				handler.Call(currentReceivedBytes, totalBytesToReceive);
			}
		}

		/// <summary>
		/// 热更新下载进度回调
		/// </summary>
		/// <param name="currentReceivedBytes">已经下载的字节数</param>
		/// <param name="totalBytesToReceive">热更新补丁包总字节大小</param>
		private void OnHotUpdateDownloadProgress(long currentReceivedBytes, long totalBytesToReceive, string currentTaskFileName)
		{
			var handler = handlerMap[HotUpdateEventType.HotUpdateDownloadProgress];
			if(handler != null)
			{
				handler.Call(currentReceivedBytes, totalBytesToReceive, currentTaskFileName);
			}

		}

		/// <summary>
		/// 开始解压下载的热更新资源回调
		/// </summary>
		private void OnStartUnzipNotify()
		{
			var handler = handlerMap[HotUpdateEventType.StartUnzipNotify];
			if(handler != null)
			{
				handler.Call();
			}

		}

        /// <summary>
        /// 磁盘空间不足
        /// </summary>
        private void OnDiskSpaceInsufficient()
        {
            var handler = handlerMap[HotUpdateEventType.DiskSpaceInsufficient];
            if (handler != null)
            {
                handler.Call();
            }
        }

		/// <summary>
		/// 解压进度回调
		/// </summary>
		/// <param name="progress">解压进度</param>
		private void OnUnzipProgress(float progress)
		{
			var handler = handlerMap[HotUpdateEventType.UnzipProgress];
			if(handler != null)
			{
				handler.Call(progress);
			}

		}

        /// <summary>
        /// 请求服务器Version.manifest文件失败回调
        /// </summary>
        /// <param name="errorInfo">错误信息</param>
        private void OnRequestRemoteVersionFail(string errorInfo)
        {
            var handler = handlerMap[HotUpdateEventType.RequestRemoteVersionFail];
            if (handler != null)
            {
                handler.Call(errorInfo);
            }
        }

		/// <summary>
		/// 请求服务器Version.manifest文件失败回调
		/// </summary>
		/// <param name="errorInfo">错误信息</param>
		/// <param name="requestAgain">再次请求</param>
		private void OnRequestRemoteVersionFail(string errorInfo, Action requestAgain)
		{
			var handler = handlerMap[HotUpdateEventType.RequestRemoteVersionFail];
			this.nextAction = requestAgain;
			if(handler != null)
			{
				handler.Call(errorInfo);
			}
		}

		/// <summary>
		/// 请求服务器Project.manifest文件失败回调
		/// </summary>
		/// <param name="errorInfo">错误信息</param>
		/// <param name="requestAgain">再次请求</param>
		private void OnRequestRemoteProjectManifestFail(string errorInfo, Action requestAgain)
		{
			var handler = handlerMap[HotUpdateEventType.RequestRemoteProjectManifestFail];
			this.nextAction = requestAgain;
			if(handler != null)
			{
				handler.Call(errorInfo);
			}
		}

		/// <summary>
		/// 下载文件失败回调
		/// </summary>
		/// <param name="task">下载失败的task</param>
		/// <param name="errorInfo">错误信息</param>
		/// <param name="downloadAgain">重新下载</param>
		private void OnHotUpdateDownloadFail(DownloadTask task, string errorInfo, Action downloadAgain)
		{
			var handler = handlerMap[HotUpdateEventType.HotUpdateDownloadFail];
			this.nextAction = downloadAgain;
			if(handler != null)
			{
				handler.Call(task.url, (int)task.errorCode, task.file, task.fileLength, errorInfo);
			}
		}

		/// <summary>
		/// 解压热更新资源失败回调
		/// </summary>
		/// <param name="status">失败原因</param>
		/// <param name="unzipAgain">重新解压</param>
		private void OnUnzipFail(UnzipStatus status, Action unzipAgain)
		{
			var handler = handlerMap[HotUpdateEventType.UnzipFail];
			this.nextAction = unzipAgain;
			if(handler != null)
			{
				handler.Call((int)status);
			}
		}
	}
}
