using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.IO;
using Framework.Download;

namespace Framework.HotUpdate
{
    internal class UnzipThread
    {
        private Thread unzipThread;
        private UnzipStatus unzipStatus = UnzipStatus.Init;
        private float _totalUnzipProgress = 0f;
        private DownloadFileTransferInfo[] _allTransferedFiles;
        private int unzipTaskIndex = 0;

        public UnzipThread(DownloadFileTransferInfo[] allTransferedFiles)
        {
            _allTransferedFiles = allTransferedFiles;
            unzipStatus = UnzipStatus.Init;
            _totalUnzipProgress = 0f;
        }

        /// <summary>
        /// 下载完成，按ver排序，由小到大排序，如果是zip文件则解压，非zip文件则覆盖
        /// 然后把文件从temp目录剪切到资源目录，并更新版号，并清空temp目录
        /// </summary>
        public void StartUnzip(Action<UnzipStatus> callback/*, Action<float> unzipProgress*/)
        {
            MainThreadRunner.Instance.StartCoroutine(StartUnzipThread(callback/*, unzipProgress*/));
        }

        public void SetTotalUnzipProgress(long bytesTransferred, long totalBytesToTransfer)
        {
            _totalUnzipProgress = (unzipTaskIndex + (float)bytesTransferred / totalBytesToTransfer) / _allTransferedFiles.Length;
        }

        private IEnumerator StartUnzipThread(Action<UnzipStatus> callback/*, Action<float> unzipProgress*/)
        {
            EventCenter.Broadcast((int)HotUpdateEventType.StartUnzipNotify);
            _totalUnzipProgress = 0f;

            unzipStatus = UnzipStatus.Init;
            unzipThread = new Thread(UnzipThreadRun);
            unzipThread.Start();

            Thread thread = unzipThread;
            while (thread.IsAlive)
            {
                EventCenter.Broadcast((int)HotUpdateEventType.UnzipProgress, _totalUnzipProgress);
                yield return null;
            }
            StopUnzipThread();

            EventCenter.Broadcast((int)HotUpdateEventType.UnzipProgress, 1f);
            yield return null;
            callback.Invoke(unzipStatus);
            unzipStatus = UnzipStatus.Init;
        }

        private void UnzipThreadRun()
        {
            Array.Sort(_allTransferedFiles);
            for (int i = 0; i < _allTransferedFiles.Length; i++)
            {
                unzipTaskIndex = i;
                unzipStatus = UnzipStatus.Processing;
                unzipStatus = UnzipOrMoveTempToAsset(_allTransferedFiles[i].path, i, _allTransferedFiles.Length);
                if (unzipStatus != UnzipStatus.Success)
                {
                    return;
                }
            }
            VersionMgr.Instance.UpdateLocalVersionData();
            VersionMgr.Instance.SaveLocalCacheVersionData();
            ClearTempDir();
        }

        private void StopUnzipThread()
        {
            if (unzipThread != null && unzipThread.IsAlive)
            {
                CLogger.Log("GameUpdateMgr::StopUnzipThread() - Abort unzip thread!!!");
                unzipThread.Abort();
                unzipThread = null;
            }
        }

        private void ClearTempDir()
        {
            string dir = FileUtils.LocalTempResRootPath;
            if (Directory.Exists(dir))
            {
                string[] allFilePaths = Directory.GetFiles(dir);
                for (int i = 0; i < allFilePaths.Length; i++)
                {
                    File.Delete(allFilePaths[i]);
                }
                Directory.Delete(dir, true);
            }
        }

        private UnzipStatus UnzipOrMoveTempToAsset(string tempFileName, int unzipTaskIndex, int maxUnzipTasks)
        {
            UnzipStatus status = UnzipStatus.Success;
            string tempFullPath = FileUtils.LocalTempResRootPath + tempFileName;
            if (tempFullPath.EndsWith(".zip"))
            {
                List<string> allUnzipFileNames;
                CLogger.Log("GameUpdateMgr::UnzipOrMoveTempToAsset() - Start unzip file, fullName=" + tempFileName);
                //EventHandler<Ionic.Zip.ExtractProgressEventArgs> extractProgress = (object sender, Ionic.Zip.ExtractProgressEventArgs e) =>
                //{
                //    _totalUnzipProgress = (unzipTaskIndex + (float)e.BytesTransferred / e.TotalBytesToTransfer) / maxUnzipTasks;
                //};
                status = ZipUtil.UnZip(tempFullPath, FileUtils.LocalResRootPath, out allUnzipFileNames);
                if (status == UnzipStatus.Success)
                {
                    CLogger.Log("GameUpdateMgr::UnzipOrMoveTempToAsset() - Unzip file success, fullName=" + tempFileName);
                    for (int i = 0; i < allUnzipFileNames.Count; i++)
                    {
                        VersionMgr.Instance.UpdateLocalCacheData(allUnzipFileNames[i]);
                    }

                    //EventCenter.Broadcast((int)HotUpdateEventType.UnzipFileNotify, allUnzipFileNames);
                    MainThreadRunner.Instance.RunOnMainThread(()=>{ EventCenter.Broadcast((int)HotUpdateEventType.UnzipFileNotify, allUnzipFileNames.ToArray()); });
                    
                }
                else
                {
                    CLogger.Log("GameUpdateMgr::UnzipOrMoveTempToAsset() - Unzip file faild, fullName=" + tempFileName);
                }
            }
            else
            {
                try
                {

                    //string dstAssetPath = FileUtils.Instance.FullPathForFile(tempFileName, ResourceType.ASSET_BUNDLE);
                    string dstAssetPath = PathManager.URL(tempFileName, AssetLoad.AssetType.eNone, false);
                    if (File.Exists(dstAssetPath))
                    {
                        File.Delete(dstAssetPath);
                    }
                    FileUtils.Instance.CheckDirExistsForFile(dstAssetPath);
                    File.Copy(tempFullPath, dstAssetPath);
                    VersionMgr.Instance.UpdateLocalCacheData(tempFileName);

                    MainThreadRunner.Instance.RunOnMainThread(() => {
                        List<string> allUnzipFileNames = new List<string>();
                        allUnzipFileNames.Add(tempFileName);
                        EventCenter.Broadcast((int)HotUpdateEventType.UnzipFileNotify, allUnzipFileNames.ToArray());
                    });
                    
                }
                catch (Exception e)
                {
                    CLogger.LogError("GameUpdateMgr::UnzipOrMoveTempToAsset() - Exception: " + e.Message);
                    if (e.Message.Contains("Disk full"))
                    {
                        status = UnzipStatus.DiskFull;
                    }
                    status = UnzipStatus.Exception;
                }
            }
            return status;
        }
    }
}
