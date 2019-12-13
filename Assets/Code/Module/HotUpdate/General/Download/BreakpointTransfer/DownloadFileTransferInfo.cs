using System;

namespace Framework.Download
{
    [Serializable]
    public class DownloadFileTransferInfo : IComparable<DownloadFileTransferInfo>
    {
        public string path;
        public string md5;
        public int size;
        public int ver;
        public long receivedSize;
        public bool IsDownloading
        {
            get { return receivedSize > 0L && receivedSize < size; }
        }
        public bool IsSuccess
        {
            get { return receivedSize == size; }
        }
        public DownloadFileTransferInfo()
        {

        }
        public DownloadFileTransferInfo(DownloadFileInfo sourceFileInfo)
        {
            this.path = sourceFileInfo.path;
            this.md5 = sourceFileInfo.md5;
            this.size = sourceFileInfo.size;
            this.ver = sourceFileInfo.ver;
        }

        public int CompareTo(DownloadFileTransferInfo other)
        {
            return this.ver - other.ver;
        }
    }
}
