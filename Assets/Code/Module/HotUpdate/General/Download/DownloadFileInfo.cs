using System;

namespace Framework.Download
{
    [Serializable]
    public class DownloadFileInfo
    {
        public string path;
        public string md5;
        public int size;
        public int ver;
    }
}
