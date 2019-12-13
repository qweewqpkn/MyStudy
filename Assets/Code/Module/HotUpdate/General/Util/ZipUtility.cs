using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;

public class ZipUtility
{
    /// <summary>  
    /// 功能：解压zip格式的文件。  
    /// </summary>  
    /// <param name="fileToUnZip">压缩文件路径</param>  
    /// <param name="zipedFolder">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>  
    /// <returns>解压是否成功</returns>  
    public static List<string> UnZipNew(string fileToUnZip, string zipedFolder)
    {
        //解压出来的文件列表 
        List<string> unzipFiles = new List<string>();
        long bytesTransferred = 0;
        long totalBytesToTransfer = 0;

        try
        {
            if (fileToUnZip == string.Empty)
            {
                throw new Exception("压缩文件不能为空！");
            }
            if (!File.Exists(fileToUnZip))
            {
                throw new FileNotFoundException("压缩文件不存在！");
            }
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹  
            if (zipedFolder == string.Empty)
                zipedFolder = fileToUnZip.Replace(Path.GetFileName(fileToUnZip), Path.GetFileNameWithoutExtension(fileToUnZip));
            if (!zipedFolder.EndsWith("/"))
                zipedFolder += "/";
            if (!Directory.Exists(zipedFolder))
                Directory.CreateDirectory(zipedFolder);
            FileInfo fileInfo = new FileInfo(fileToUnZip);
            totalBytesToTransfer = fileInfo.Length;
            using (var s = new ZipInputStream(File.OpenRead(fileToUnZip)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        Directory.CreateDirectory(zipedFolder + directoryName);
                    }
                    if (directoryName != null && !directoryName.EndsWith("/"))
                    {
                    }
                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(zipedFolder + theEntry.Name))
                        {
                            
                            int size;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                    bytesTransferred += size;
                                    Framework.HotUpdate.GameUpdateMgr.Instance.OnUnzipProgress(bytesTransferred, totalBytesToTransfer);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        //记录导出的文件 
                        unzipFiles.Add(theEntry.Name);
                    }
                }
            }
        }
        catch (Exception e)
        {

        }
        return unzipFiles;
    }
}