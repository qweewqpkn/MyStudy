/*-------------------------------------------------------------------
// Copyright (C)
//
// Module: VersionMgr
// Author: huangxin
// Date: 2017.08.29
// Description: Game version management.
//-----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework.HotUpdate
{

    public class VersionMgr : Singleton<VersionMgr>
    {
        private BaseVersionData _localVerCode;
        private BaseVersionData _remoteVerCode;
        private LocalCacheVersionDataMgr _localCacheVerDataMgr;
        private ResponseProjectManifestData _projectManifestInfo;

        public string localVersion { get { return _localVerCode.Version; } }
        public string localVersionName { get { return _projectManifestInfo.versionName; } }
        public BaseVersionData remoteVerCode { get { return _remoteVerCode; } }
        public BaseVersionData localCacheVerCode { get { return _localCacheVerDataMgr.VersionData; } }
        
        public ResponseProjectManifestData projectManifestInfo { get { return _projectManifestInfo; } }

        public bool IsClientNew { get { return (_localVerCode.Major > _remoteVerCode.Major) ||
                                               (_localVerCode.Major == _remoteVerCode.Major && _localVerCode.Minor > _remoteVerCode.Minor) ||
                                               (_localVerCode.Major == _remoteVerCode.Major && _localVerCode.Minor == _remoteVerCode.Minor && _localVerCode.Revision >= _remoteVerCode.Revision); } }

        public bool IsPackageNew { get { return _localVerCode.Major >= _remoteVerCode.Major; } }

        public bool IsNeedHotUpdate { get { return _localVerCode.Major == _remoteVerCode.Major && _localVerCode.Revision < _remoteVerCode.Revision; } }

        public bool LoadLocalVersionCode()
        {
            BaseVersionData localVersion = new BaseVersionData();
            string localVerFilePath = FileUtils.LocalResRootPath + HotUpdateDefs.kPackageVersionFile;

            bool isCorveringInstallation = false;

            if (File.Exists(localVerFilePath))
            {
                localVersion.Version = JsonUtility.FromJson<ResponseVersionManifestData>(File.ReadAllText(localVerFilePath)).version;
                CLogger.Log("VersionMgr::LoadLocalVersionCode() - Load local version code: " + localVersion.Version);
            }

            var txt = Resources.Load(HotUpdateDefs.kPackageVersionFile);
            if (txt == null)
            {
                CLogger.LogError("VersionMgr::LoadLocalVersionCode() - 缺少包版本文件：Resources/version.manifest!!!");
                return false;
            }
            BaseVersionData packageVersion = new BaseVersionData();
            packageVersion.Version = JsonUtility.FromJson<ResponseVersionManifestData>(txt.ToString()).version;
            CLogger.Log("VersionMgr::LoadLocalVersionCode() - Load package version code: " + packageVersion.Version);
            if ((packageVersion.Major > localVersion.Major) ||
                (packageVersion.Major == localVersion.Major && packageVersion.Minor > localVersion.Minor) ||
                (packageVersion.Major == localVersion.Major && packageVersion.Minor == localVersion.Minor && packageVersion.Revision > localVersion.Revision))
            {
                // 删除本地热更新的文件缓存
                DeleteAllLocal();

                FileUtils.Instance.CheckDirExistsForFile(localVerFilePath);
                File.WriteAllText(localVerFilePath, JsonUtility.ToJson(new ResponseVersionManifestData() { version = packageVersion.Version }));
                _localVerCode = packageVersion;

                isCorveringInstallation = true;
            }
            else if (packageVersion.Major < localVersion.Major)
            {
                // 玩家安装了2.0.1的版本后又覆盖安装了1.0.1的版本会出现这种情况，应该删除本地缓存热更大版本
                DeleteAllLocal();

                _localVerCode = packageVersion;

                isCorveringInstallation = true;
            }
            else
                _localVerCode = localVersion;

            Resources.UnloadAsset(txt);

            PlayerPrefs.SetInt("isCorveringInstallation", isCorveringInstallation ? 1 : 0);

            CLogger.Log("VersionMgr::LoadLocalVersionCode() - Client version code: " + _localVerCode.Version);

            //CDebugInfo debug_info = GameObject.Find("UIRoot/Canvas/Debug").GetComponent<CDebugInfo>();
            //debug_info.m_HotUpdateVersion.text = "HUVer: " + _localVerCode.Version;

            return true;
        }

        private void SaveLocalVersionCode()
        {
            ResponseVersionManifestData data = new ResponseVersionManifestData();
            data.version = _localVerCode.Version;
            string localVerFilePath = FileUtils.LocalResRootPath + HotUpdateDefs.kPackageVersionFile;
            File.WriteAllText(localVerFilePath, JsonUtility.ToJson(data));
        }

        private void LoadLocalCacheVersionData()
        {
            string localVerFilePath = FileUtils.LocalResRootPath + HotUpdateDefs.kLocalCacheVersionDataFile;
            _localCacheVerDataMgr = new LocalCacheVersionDataMgr();
            if (File.Exists(localVerFilePath))
            {
                _localCacheVerDataMgr.Init(File.ReadAllText(localVerFilePath));
                CLogger.Log("VersionMgr::LoadLocalCacheVersionData() - LocalCacheVersion: " + _localCacheVerDataMgr.VersionData.Version);
                if (_localCacheVerDataMgr.VersionData.Major != _localVerCode.Major)
                {
                    DeleteAllLocal();
                    _localCacheVerDataMgr.Clear();
                    CLogger.Log("VersionMgr::LoadLocalCacheVersionData() - Delete all local cache version data.");
                }
            }
            else
            {
                CLogger.Log("VersionMgr::LoadLocalCacheVersionData() - LocalCacheVersionData is empty.");
                CLogger.Log("VersionMgr::LoadLocalCacheVersionData() - LocalCacheVersion: " + _localCacheVerDataMgr.VersionData.Version);
            }
        }

        public void SaveLocalCacheVersionData()
        {
            string data = _localCacheVerDataMgr.ToJson();
            string path = FileUtils.LocalResRootPath + HotUpdateDefs.kLocalCacheVersionDataFile;
            FileUtils.Instance.CheckDirExistsForFile(path);
            FileUtils.Instance.WriteFile(data, path);
            CLogger.Log("VersionMgr::SaveLocalCacheVersionData() - SaveLocalCacheVersionData:" + path);
        }

        public bool IsLocalCacheFile(string file)
        {
            return _localCacheVerDataMgr != null && _localCacheVerDataMgr.HasCache(file);
        }

        public void UpdateLocalCacheData(string file)
        {
            _localCacheVerDataMgr.AddCacheFile(file);
            CLogger.Log("VersionMgr::UpdateLocalCacheData() - UpdateLocalCacheData: " + file);
        }

        public void UpdateLocalVersionData()
        {
            _localCacheVerDataMgr.VersionData.Version = _remoteVerCode.Version;
            _localVerCode.Version = _remoteVerCode.Version;
            SaveLocalVersionCode();
            CLogger.Log("VersionMgr::UpdateLocalVersionData() - UpdateLocalVersionData:" + _localCacheVerDataMgr.VersionData.Version);
        }

        public void DeleteAllLocal()
        {
            string dir = FileUtils.LocalResRootPath;
            if (Directory.Exists(dir))
            {
                FileUtils.Instance.DeleteDirectory(dir);
            }
        }

        /// <summary>
        /// 加载服务器的版本号信息
        /// </summary>
        /// <param name="versionManifestUrl"></param>
        /// <param name="finishCallback"></param>
        /// <returns></returns>
        public IEnumerator LoadRemoteVersionCode(string versionManifestUrl, Action<string> failedCallback, Action<ResponseVersionManifestData> finishCallback)
        {
            // 请求远程最新版本信息
            if (versionManifestUrl.Contains("?"))
                versionManifestUrl += "&__cdn_asset_version__=" + (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
            else
                versionManifestUrl += "?__cdn_asset_version__=" + (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
            WWW www = new WWW(versionManifestUrl);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
				CLogger.LogError("VersionMgr::LoadRemoteVersionCode() - request remote_version.manifest error: " + versionManifestUrl + "     " + www.error);                
                if (failedCallback != null)
                {
                    CLogger.Log("VersionMgr::LoadRemoteVersionCode() - Request version.manifest failed. Please try again!");
                    failedCallback(www.error);
                }
                www.Dispose();
                yield break;
            }
            // 解析远程最新版本信息
            _remoteVerCode = new BaseVersionData();
            ResponseVersionManifestData manifestData = null;
            try
            {
                manifestData = JsonUtility.FromJson<ResponseVersionManifestData>(www.text);
                _remoteVerCode.Version = manifestData.version;
            } catch (Exception e)
            {
                CLogger.LogError("VersionMgr::LoadRemoteVersionCode() - parse version.manifest exception:" + e.Message);                
                if (failedCallback != null)
                {
                    CLogger.Log("VersionMgr::LoadRemoteVersionCode() - Request version.manifest failed. Please try again!");
                    failedCallback(e.Message);
                }
                www.Dispose();
                yield break;
            }

            manifestData = manifestData != null ? manifestData : new ResponseVersionManifestData();
            CLogger.Log("VersionMgr::LoadRemoteVersionCode() - LoadRemoteVersionCode: " + _remoteVerCode.Version);
            www.Dispose();
            www = null;

            // 加载本地缓存版本文件信息
            if (_localCacheVerDataMgr == null)
                LoadLocalCacheVersionData();
            
            if (finishCallback != null){
                finishCallback(manifestData);
            }
        }

        public IEnumerator RequestRemoteVersionCode(string versionManifestUrl, Action<string> failedCallback, Action<ResponseVersionManifestData> finishCallback)
        {
            // 请求远程最新版本信息
            if (versionManifestUrl.Contains("?"))
                versionManifestUrl += "&__cdn_asset_version__=" + (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
            else
                versionManifestUrl += "?__cdn_asset_version__=" + (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
            WWW www = new WWW(versionManifestUrl);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                CLogger.LogError("VersionMgr::RequestRemoteVersionCode() - request remote_version.manifest error: " + www.error);                
                if (failedCallback != null)
                {
                    CLogger.Log("VersionMgr::RequestRemoteVersionCode() - Request version.manifest failed. Please try again!");
                    failedCallback(www.error);
                }
                www.Dispose();
                yield break;
            }
            // 解析远程最新版本信息
            ResponseVersionManifestData manifestData = null;
            string remoteVersion = string.Empty;
            try
            {
                manifestData = JsonUtility.FromJson<ResponseVersionManifestData>(www.text);
                remoteVersion = manifestData.version;
                
            }catch (Exception e)
            {
                CLogger.LogError("VersionMgr::RequestRemoteVersionCode() - parse version.manifest exception: " + e.Message);                
                if (failedCallback != null)
                {
                    CLogger.Log("VersionMgr::RequestRemoteVersionCode() - Request version.manifest failed. Please try again!");
                    failedCallback(e.Message);
                }
                www.Dispose();
                yield break;
            }

            manifestData = manifestData != null ? manifestData : new ResponseVersionManifestData();
            CLogger.Log("VersionMgr::RequestRemoteVersionCode() - RequestRemoteVersionCode: " + remoteVersion);
            www.Dispose();
            www = null;

            if (finishCallback != null){
                finishCallback(manifestData);
            }
        }

        /// <summary>
        /// 请求版本更新信息
        /// </summary>
        /// <returns></returns>
        public IEnumerator RequestVersionUpdateInfo(string projectManifestUrl, Action<string> failedCallback, Action finishCallback)
        {
            if (projectManifestUrl.Contains("?"))
                projectManifestUrl += "&__cdn_asset_version__=" + (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
            else
                projectManifestUrl += "?__cdn_asset_version__=" + (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1))).TotalSeconds;
            WWW www = new WWW(projectManifestUrl);
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                CLogger.LogError("VersionMgr::RequestVersionUpdateInfo() - request project.manifest error: " + www.error);                
                if (failedCallback != null)
                {
                    CLogger.Log("VersionMgr::RequestVersionUpdateInfo() - request project.manifest again!");
                    failedCallback(www.error);
                }
                www.Dispose();
                yield break;
            }

            // 解析版本更新信息
            try
            {
                _projectManifestInfo = JsonUtility.FromJson<ResponseProjectManifestData>(www.text);
            }catch (Exception e)
            {
                CLogger.LogError("VersionMgr::RequestVersionUpdateInfo() - parse project.manifest exception: " + e.Message);                
                if (failedCallback != null)
                {
                    CLogger.Log("VersionMgr::RequestVersionUpdateInfo() - request project.manifest again!");
                    failedCallback(e.Message);
                }
                www.Dispose();
                yield break;
            }
            if (_projectManifestInfo == null || string.IsNullOrEmpty(_projectManifestInfo.appUrl) ||
                string.IsNullOrEmpty(_projectManifestInfo.packageUrl))
            {
                CLogger.Log("VersionMgr::RequestVersionUpdateInfo() - project.manifest is empty!!!!");                
                if (failedCallback != null)
                {
                    failedCallback("VersionMgr::RequestVersionUpdateInfo() - project.manifest is empty");
                }
                www.Dispose();
                yield break;
            }
            CLogger.Log("VersionMgr::RequestVersionUpdateInfo() - Request remote project manifest:" + www.text);

            www.Dispose();

            if (finishCallback != null)
                finishCallback();
        }
    }
}
