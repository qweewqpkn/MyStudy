using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    //拷贝源目录所有文件(包含子目录的文件)到目标目录
    public static void CopyTo(string sourceDir, string targetDir, string filter = "*",string postfix = "")
    {
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
                if(postfix == "")
                {
                    File.Copy(files[i], string.Format("{0}/{1}", targetDir, Path.GetFileName(files[i])));
                }
                else
                {
                    File.Copy(files[i], string.Format("{0}/{1}.{2}", targetDir, Path.GetFileName(files[i]), postfix));
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
                CopyTo(curDirs[i], targetDir + "/" + path, filter, postfix);
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
}
