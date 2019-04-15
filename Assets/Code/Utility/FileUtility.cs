using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class FileUtility{

    //创建一个目录
    public static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static void DeleteDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    //拷贝源目录所有文件(包含子目录的文件)到目标目录
    public static void CopyTo(string sourceDir, string targetDir,string filter = "*",string postfix = "", string rootPath = "")
    {
        rootPath = rootPath.Replace("\\", "/");
        targetDir = targetDir.Replace("\\", "/");
        sourceDir = sourceDir.Replace("\\", "/");

        if (!Directory.Exists(sourceDir))
        {
            Debug.LogError("sourcePath is not exist");
            return;
        }

        if (Directory.Exists(targetDir))
        {
            Directory.Delete(targetDir, true);
        }
        Directory.CreateDirectory(targetDir);

        string[] files = Directory.GetFiles(sourceDir, filter);
        if (files != null)
        {
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = "";
                if (!string.IsNullOrEmpty(rootPath))
                {
                    string moduleName = targetDir.Replace(rootPath + "/", "").Split('/')[0];
                    fileName = targetDir.Replace(rootPath + "/" + moduleName, "");
                    //fileName可能为空，在当前模块目录下有文件时
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        //移除斜杠,替换斜杠
                        fileName = fileName.Remove(0, 1).Replace("/", "_");
                        fileName = fileName + "_" + Path.GetFileName(files[i]);
                    }
                    else
                    {
                        fileName = Path.GetFileName(files[i]);
                    }
                }
                else
                {
                    fileName = Path.GetFileName(files[i]);
                }

                if (postfix == "")
                {
                    File.Copy(files[i], string.Format("{0}/{1}", targetDir, fileName));
                }
                else
                {
                    File.Copy(files[i], string.Format("{0}/{1}.{2}", targetDir, fileName, postfix));
                }
            }
        }

        string[] curDirs = Directory.GetDirectories(sourceDir);
        if (curDirs != null)
        {
            for (int i = 0; i < curDirs.Length; i++)
            {
                curDirs[i] = curDirs[i].Replace("\\", "/");
                string path = curDirs[i].Substring(curDirs[i].LastIndexOf("/") + 1);
                CopyTo(curDirs[i], targetDir + "/" + path, filter, postfix, rootPath);
            }
        }
    }

    public static void ClearEmptyDirectory(string path)
    {
        string[] files = Directory.GetFiles(path);
        string[] directories = Directory.GetDirectories(path);
        if(files.Length == 0 && directories.Length == 0)
        {
            Directory.Delete(path);
        }

        for(int i = 0; i < directories.Length; i++)
        {
            ClearEmptyDirectory(directories[i]);
        }
    }

    public static string MD5File(string path)
    {
        string hash = "";
        if (File.Exists(path))
        {
            FileStream fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            MD5 md5 = MD5.Create();
            hash = System.BitConverter.ToString(md5.ComputeHash(fs));
            fs.Close();
            fs.Dispose();
        }
        return hash;
    }

    public static long GetFileLength(string path)
    {
        if (File.Exists(path))
        {
            FileStream fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
            long length = fs.Length;
            fs.Close();
            fs.Dispose();
            return length;
        }
        else
        {
            return 0;
        }
    }

    //判断目录下是否存在指定名字的文件
    public static bool ExistFile(string directoryPath, string name, out string path, string filter = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
    {
        path = "";
        string[] files = Directory.GetFiles(directoryPath, filter, searchOption);
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(files[i]);
            if (fileName == name)
            {
                path = files[i];
                return true;
            }
        }

        return false;
    }
}
