using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.Download
{
    public class BreakpointTransferMgr
    {
        private BreakpointTransferInfo _transferInfo = null;
        public BreakpointTransferInfo transferInfo
        {
            get { return _transferInfo; }
        }
        private string _transferInfoFilePath;
        //文件会首先下载保存在这个临时目录中
        private string _tempResRootPath;

        // 总更新包大小
        private long _totalBytesToReceive;
        public long TotalBytesToReceive { get { return _totalBytesToReceive; } }

        // 已经下载的文件大小
        private long _totalReceivedBytes;
        public long TotalReceivedBytes { get { return _totalReceivedBytes; } }

        public List<DownloadFileInfo> _downloadList;

        private Dictionary<string, DownloadFileTransferInfo> _downloadFileInfoDict;

        private object _locker = new object();

        public BreakpointTransferMgr(string tempResRootPath, string transferInfoFileName)
        {
            _tempResRootPath = tempResRootPath;
            _transferInfoFilePath = tempResRootPath + transferInfoFileName;
        }

        public void Dispose()
        {
            _transferInfo = null;
            if (_downloadList != null)
            {
                _downloadList.Clear();
                _downloadList = null;
            }
            if (_downloadFileInfoDict != null)
            {
                _downloadFileInfoDict.Clear();
                _downloadFileInfoDict = null;
            }
        }

        //开始续传，并返回需要下载的文件列表
        public List<string> StartContinueTransfer(List<DownloadFileInfo> downloadList)
        {
            _totalBytesToReceive = 0L;
            _totalReceivedBytes = 0L;
            _downloadList = downloadList;
            _downloadFileInfoDict = new Dictionary<string, DownloadFileTransferInfo>();

            HandlerContinueTransfer();
            return CalcurateNeedDownloadFileList();
        }

        private void HandlerContinueTransfer()
        {
            ReadTransferProgress();
            Combine();

            for (int i = 0; i < _transferInfo.allFiles.Length; i++)
            {
                _downloadFileInfoDict.Add(_transferInfo.allFiles[i].path, _transferInfo.allFiles[i]);
            }
        }

        private void Combine()
        {
            List<DownloadFileTransferInfo> list = new List<DownloadFileTransferInfo>();
            if (_transferInfo.allFiles == null || _transferInfo.allFiles.Length == 0)
            {
                for (int i = 0; i < _downloadList.Count; i++)
                {
                    list.Add(new DownloadFileTransferInfo(_downloadList[i]));
                }
            }
            else
            {
                DownloadFileInfo hotFileInfo;
                DownloadFileTransferInfo fileTransferInfo;
                bool isFind = false;
                for (int i = 0; i < _downloadList.Count; i++)
                {
                    hotFileInfo = _downloadList[i];

                    for (int j = 0; j < _transferInfo.allFiles.Length; j++)
                    {
                        fileTransferInfo = _transferInfo.allFiles[j];
                        if (hotFileInfo.path == fileTransferInfo.path)
                        {
                            if (hotFileInfo.md5 != fileTransferInfo.md5)
                            {
                                //不同则删除重新下载
                                DeleteTempFile(hotFileInfo.path);
                            }
                            else
                            {
                                isFind = true;
                                list.Add(fileTransferInfo);
                            }
                            break;
                        }
                    }
                    if (!isFind)
                    {
                        list.Add(new DownloadFileTransferInfo(hotFileInfo));
                    }
                }
            }
            _transferInfo.allFiles = list.ToArray();
        }

        private List<string> CalcurateNeedDownloadFileList()
        {
            List<string> list = new List<string>();
            DownloadFileTransferInfo info;
            for (int i = 0; i < _transferInfo.allFiles.Length; i++)
            {
                info = _transferInfo.allFiles[i];
                _totalBytesToReceive += info.size;
                _totalReceivedBytes += info.receivedSize;
                if (!info.IsSuccess)
                {
                    list.Add(info.path);
                }
            }
            //保持至少有一个文件需要下载，走前端的热更流程，此文件本地肯定是已经下载完成的，不会真的去下的，只是为了能走前端的流程
            if (list.Count == 0 && _transferInfo.allFiles.Length > 0)
            {
                list.Add(_transferInfo.allFiles[0].path);
            }
            return list;
        }

        public DownloadFileTransferInfo GetDownloadFileInfo(string filePath)
        {
            if (_downloadFileInfoDict.ContainsKey(filePath))
            {
                return _downloadFileInfoDict[filePath];
            }
            return null;
        }

        public void ReadTransferProgress()
        {
            lock (_locker)
            {
                if (File.Exists(_transferInfoFilePath))
                {
                    string json = File.ReadAllText(_transferInfoFilePath);
                    _transferInfo = JsonUtility.FromJson<BreakpointTransferInfo>(json);
                    if (_transferInfo == null)
                        CLogger.LogError("解析本地断点续传记录json文件失败,json string=" + json);
                }

                if (_transferInfo == null)
                    _transferInfo = new BreakpointTransferInfo();
            }
        }

        public void SaveTransferProgress(string msg)
        {
			CLogger.Log ("BreakpointTransferMgr::SaveTransferProgress() msg====" + msg);
            lock (_locker)
            {
                CLogger.Log("BreakpointTransferMgr::SaveTransferProgress() - Save Transfer Progress file path: " + _transferInfoFilePath);
                FileUtils.Instance.CheckDirExistsForFile(_transferInfoFilePath);
                string json = JsonUtility.ToJson(_transferInfo);
                if (!string.IsNullOrEmpty(json))
                    File.WriteAllText(_transferInfoFilePath, json);
            }
        }

        public void DeleteTempFile(string tempPath)
        {
            string path = _tempResRootPath + tempPath;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        } 

        /// <summary>
        /// 更新文件传输进度信息
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="receivedSize"></param>
        public void UpdateFileTransferProgress(string filePath, long receivedSize)
        {
            if (_downloadFileInfoDict.ContainsKey(filePath))
            {
                UpdateFileTransferProgress(_downloadFileInfoDict[filePath], receivedSize);
                _totalReceivedBytes += receivedSize;
            }
        }

        /// <summary>
        /// 更新文件传输进度信息
        /// </summary>
        /// <param name="file"></param>
        /// <param name="receivedSize"></param>
        public void UpdateFileTransferProgress(DownloadFileTransferInfo file, long receivedSize)
        {
            _totalReceivedBytes += receivedSize - file.receivedSize;
            file.receivedSize = receivedSize;
        }

        public float GetTotalTransferProgress()
        {
            return (float)((double)_totalReceivedBytes / (double)_totalBytesToReceive);
        }

    }
}
