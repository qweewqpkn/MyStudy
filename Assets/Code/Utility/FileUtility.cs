using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class FileUtility{

    public delegate string ReNameAction(string targetPath, string filePath);

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
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    //拷贝源目录所有文件(包含子目录的文件)到目标目录
    //sourceDir 原目录
    //targetDir 目标目录
    //filter 原始目录的过滤类型如*.txt
    //postfix 目标文件的增加的后缀名
    //action 修改拷贝时文件的名字
    public static void CopyTo(string sourceDir, string targetDir,string filter = "*",string postfix = "", ReNameAction action = null)
    {
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
                if (action != null)
                {
                    fileName = action(targetDir, files[i]);
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
                CopyTo(curDirs[i], targetDir + "/" + path, filter, postfix, action);
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

    static public void ShowAndSelectFileInExplorer(string path)
    {
        path = path.Replace('/', '\\');
        string arg = string.Format(@"/select,{0}", path);
        System.Diagnostics.Process.Start("explorer.exe", arg);
    }
}
