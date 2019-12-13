using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Ionic.Zip;
using UnityEngine;
//using Logger = Framework.CLogger;

//using ICSharpCode.SharpZipLib.Checksums;
//using ICSharpCode.SharpZipLib.Zip;

namespace Framework
{
    public enum UnzipStatus
    {
        Init=1,
        Processing=2,
        Success=3,
        DiskFull=4,
        ThreadAbort=5,
        Exception=6,
        NotFound=7
    }

    public static class ZipUtil
    {
        public static UnzipStatus UnZip(string fileToUnZip, string zipedFolder, string password, out List<string> extractFileNames)
        {
            extractFileNames = new List<string>();
            //FileStream fs = null;
            //ZipInputStream zipStream = null;
            //ZipEntry ent = null;
            string path = null;
            if (zipedFolder.EndsWith("/") || zipedFolder.EndsWith("\\"))
            {
                zipedFolder = zipedFolder.Substring(0, zipedFolder.Length - 1);
            }

            //Logger.LogInfo("1-->" + fileToUnZip);
            if (!File.Exists(fileToUnZip))
                return UnzipStatus.NotFound;

            //Logger.LogInfo("2-->" + zipedFolder);
            if (!Directory.Exists(zipedFolder))
                Directory.CreateDirectory(zipedFolder);

            //try
            //{
            //    using (ZipFile zip = ZipFile.Read(fileToUnZip))
            //    {
            //        zip.ExtractProgress += extractProgress;
            //        if (zip.Entries.Count > 0)
            //        {
            //            foreach (ZipEntry entry in zip.Entries)
            //            {
            //                if (Thread.CurrentThread.ThreadState == ThreadState.AbortRequested)
            //                {
            //                    CLogger.Log("unzip abort:->the thread is abored");
            //                    return UnzipStatus.ThreadAbort;
            //                }
            //                if (!string.IsNullOrEmpty(entry.FileName))
            //                {
            //                    path = Path.Combine(zipedFolder, entry.FileName);
            //                    path = path.Replace('\\', '/');

            //                    //Logger.LogInfo("3-->" + path);

            //                    if (path.EndsWith("/"))
            //                    {
            //                        if (!Directory.Exists(path))
            //                        {
            //                            //Logger.LogInfo("4-->" + path);
            //                            Directory.CreateDirectory(path);
            //                        }
            //                        continue;
            //                    }
            //                    else
            //                    {
            //                        string dir = path.Substring(0, path.LastIndexOf("/"));
            //                        if (!Directory.Exists(dir))
            //                        {
            //                            Directory.CreateDirectory(dir);
            //                        }
            //                    }
            //                    //Logger.LogInfo("5-->" + path);
            //                    extractFileNames.Add(entry.FileName);

            //                    //entry.Extract(zipedFolder, ExtractExistingFileAction.OverwriteSilently);

            //                    FileStream fs = new FileStream(path, FileMode.Create);
            //                    try
            //                    {
            //                        entry.Extract(fs);
            //                    }
            //                    finally
            //                    {
            //                        fs.Flush();
            //                        fs.Close();
            //                    }
            //                }
            //            }
            //        }
            //    }

            //}
            //catch (Exception e)
            //{
            //    CLogger.LogError(e.Message + "\n" + e.StackTrace);
            //    CLogger.LogError("unzip faild-->zipFile:" + fileToUnZip + "\n unzip progress:" + path);
            //    extractFileNames.Clear();
            //    if (e.Message.Contains("Disk full"))
            //    {
            //        return UnzipStatus.DiskFull;
            //    }
            //    return UnzipStatus.Exception;
            //}
            //finally
            //{
            //    GC.Collect();
            //    GC.Collect(1);
            //}

            try
            {
                extractFileNames = ZipUtility.UnZipNew(fileToUnZip, zipedFolder);
            }
            catch (Exception e)
            {
                CLogger.LogError(e.Message + "\n" + e.StackTrace);
                CLogger.LogError("unzip faild-->zipFile:" + fileToUnZip + "\n unzip progress:" + path);
                return UnzipStatus.Exception;
            }
            return UnzipStatus.Success;
        }

        /// <summary>   
        /// 解压功能(解压压缩文件到指定目录)   
        /// </summary>   
        /// <param name="fileToUnZip">待解压的文件</param>   
        /// <param name="zipedFolder">指定解压目标目录</param>   
        /// <returns>解压结果</returns>   
        public static UnzipStatus UnZip(string fileToUnZip, string zipedFolder, out List<string> extractFileNames)
        {
            UnzipStatus result = UnZip(fileToUnZip, zipedFolder, null, out extractFileNames);
            return result;
        }
    }
}
