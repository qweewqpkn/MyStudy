using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
public class MD5Hash
{
    public static string GetFileMD5_lowercase(string path)
    {
        return GetFileMD5(path).ToLower();
    }

    public static string Get(string path)
    {
        return GetFileMD5(path);
    }

    // md5服务
    static System.Security.Cryptography.MD5 md5Service = null;

    // 获取文件的MD5
    private static string GetFileMD5(string filename)
    {
        //计算文件的MD5->md5
        if (md5Service == null)
        {
            md5Service = new System.Security.Cryptography.MD5CryptoServiceProvider();
        }
        FileStream fs = new FileStream(filename, FileMode.Open);
        byte[] retVal = md5Service.ComputeHash(fs);
        fs.Close();

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < retVal.Length; i++)
        {
            sb.Append(retVal[i].ToString("x2"));
        }

        return sb.ToString();
    }
}