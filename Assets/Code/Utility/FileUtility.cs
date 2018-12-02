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
    public static void CopyTo(string sourceDir, string targetDir)
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

        string[] files = Directory.GetFiles(sourceDir);
        if (files != null)
        {
            for (int i = 0; i < files.Length; i++)
            {
                File.Copy(files[i], targetDir + "/" + Path.GetFileName(files[i]));
            }
        }

        string[] curDirs = Directory.GetDirectories(sourceDir);
        if (curDirs != null)
        {
            for (int i = 0; i < curDirs.Length; i++)
            {
                string path = curDirs[i].Substring(curDirs[i].LastIndexOf("\\") + 1);
                CopyTo(curDirs[i], targetDir + "/" + path);
            }
        }
    }
}
