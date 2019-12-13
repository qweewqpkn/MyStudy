using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

namespace Framework.Download
{
    public class DownloadMgr
    {
        private readonly float kTickInterval = 0.03f;

        private DownloadThread _downloadThread;
        private Queue<DownloadTask> _pendingTasks;
        private Queue<DownloadTask> _finishedTasks;
        private Queue<DownloadTask> _tempTasks;

        private BreakpointTransferMgr _transferMgr;

        public string CurrentTaskFileName { get { return _downloadThread.CurrentTaskFileName; } }
        public long CurrentTaskReceivedBytes { get { return _downloadThread.CurrentReceivedBytes; } }
        public long CurrentTaskTotalBytes { get { return _downloadThread.CurrentTaskTotalBytes; } }

        public long TotalBytesToReceive { get { return _transferMgr.TotalBytesToReceive; } }
        public long TotalReceivedBytes { get { return _transferMgr.TotalReceivedBytes; } }

        public DownloadFileTransferInfo[] AllTransferFiles { get { return _transferMgr.transferInfo.allFiles; } }

        public DownloadMgr(string tempResRootPath, string transferInfoFileName)
        {
            _transferMgr = new BreakpointTransferMgr(tempResRootPath, transferInfoFileName);
            _pendingTasks = new Queue<DownloadTask>();
            _finishedTasks = new Queue<DownloadTask>();
            _tempTasks = new Queue<DownloadTask>();
            _downloadThread = new DownloadThread(_transferMgr, _pendingTasks, _finishedTasks);
        }

        public void Dispose()
        {
            _pendingTasks.Clear();
            _finishedTasks.Clear();
            _tempTasks.Clear();
            _transferMgr.Dispose();
        }

        public void StartService()
        {
            _finishedTasks.Clear();
            _downloadThread.Start();
            Scheduler.Instance.AddListener(this.Tick, kTickInterval);
        }

        public void StopService()
        {
            _downloadThread.Stop();
            Scheduler.Instance.RemoveListener(this.Tick);
        }

        public List<string> StartContinueTransfer(List<DownloadFileInfo> downloadList)
        {
            return _transferMgr.StartContinueTransfer(downloadList);
        }

        public void SaveTransferProgress(string msg)
        {
            _transferMgr.SaveTransferProgress(msg);
        }

        private void Tick()
        {
            _tempTasks.Clear();
            lock (_finishedTasks)
            {
                while (_finishedTasks.Count > 0)
                {
                    _tempTasks.Enqueue(_finishedTasks.Dequeue());
                }
            }
            while (_tempTasks.Count > 0)
            {
                DownloadTask task = _tempTasks.Dequeue();
                if (task != null)
                {
                    task.onFinish(task);
                }
            }
        }

        public void AsyncDownloadList(List<string> list, string remoteDir, string localDir, Action<DownloadTask> onFinish, List<string> exts = null)
        {
            lock (_pendingTasks)
            {
                int count = list.Count;
                string url;
                for (int i = 0; i < count; ++i)
                {
                    url = list[i];
                    DownloadTask task = new DownloadTask();
                    task.file = url;
                    task.storagePath = CombineUrl(localDir, url);
                    task.url = CombineUrl(remoteDir, url);
                    task.onFinish = onFinish;
                    if (!_pendingTasks.Contains(task))
                    {
                        _pendingTasks.Enqueue(task);
                    }

                    DownloadFileTransferInfo info = _transferMgr.GetDownloadFileInfo(task.file);
                    task.md5 = info.md5;
                    Debug.Log("<color='#ff0000'>[正在下载文件: " + task.url + "</color>");
                }

                if (_downloadThread.isWaitting)
                {
                    _downloadThread.Notify();
                }
            }
        }

        private string CombineUrl(string rawUrl1, string rawUrl2)
        {
            string trimUrl1 = rawUrl1.TrimEnd('/', '\\');
            string trimUrl2 = rawUrl2.TrimStart('/', '\\');
            string result = string.Format("{0}/{1}", trimUrl1, trimUrl2);
            return result;
        }

        private string GetRemoteUrl(string url, string ext)
        {
            int index = url.LastIndexOf(".");
            if (index < 0)
                return url;
            string result;
            string prev = url.Substring(0, index);
            string next = url.Substring(index);
            result = prev + ext + next;
            return result;
        }

        public void AsyncDownload(string url, string localPath, Action<DownloadTask> onFinish)
        {
            DownloadTask task = new DownloadTask();
            task.url = url;
            task.storagePath = localPath;
            task.onFinish = onFinish;
            DownloadFileTransferInfo info = _transferMgr.GetDownloadFileInfo(url);
            task.md5 = info.md5;

            EnqueueTask(task);
        }

        private void EnqueueTask(DownloadTask task)
        {
            lock (_pendingTasks)
            {
                if (!_pendingTasks.Contains(task))
                {
                    _pendingTasks.Enqueue(task);
                }

                if (_downloadThread.isWaitting)
                {
                    _downloadThread.Notify();
                }
            }
        }
    }

}
