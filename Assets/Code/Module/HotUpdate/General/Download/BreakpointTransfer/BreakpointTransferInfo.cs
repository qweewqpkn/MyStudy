using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Download
{
    [Serializable]
    public class BreakpointTransferInfo
    {
        //public string version;
        public DownloadFileTransferInfo[] allFiles;

        public BreakpointTransferInfo()
        {

        }

        public BreakpointTransferInfo(DownloadFileInfo[] allNeedHotUpdateFiles/*, string version*/)
        {
            //this.version = version;
            allFiles = new DownloadFileTransferInfo[allNeedHotUpdateFiles.Length];
            for (int i = 0; i < allNeedHotUpdateFiles.Length; i++)
            {
                allFiles[i] = new DownloadFileTransferInfo(allNeedHotUpdateFiles[i]);
            }
        }
    }
}
