using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Framework.Download
{
    public enum DownloadStatus
    {
        Init,
        Fail,
        Complete
    }

    public enum DownloadError
    {
        NetworkDisconnect=1,
        NotFound=2,
        ServerMaintenance=3,
        DiskFull=4,
        Timeout=5,
        Unknown=6
    }

    public class DownloadTask : IEquatable<DownloadTask>
	{ 
		public string file;
		//address
		public string url;      
		public string storagePath; 
		public string md5;

		//callback
		//public Action onProgress;
        public Action<DownloadTask> onFinish;

		//length 
		public long fileLength;
		public long receivedLength;

		//
		public DownloadStatus  status;   //0 success 1 error 
		public DownloadError   errorCode;

        public bool Equals(DownloadTask other)
        {
            return file == other.file;
        }
    }
}