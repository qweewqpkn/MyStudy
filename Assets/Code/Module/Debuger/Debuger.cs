using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Debuger
{
    public enum OutputTarget
    {
        eNone,
        eConsole = 1, //控制台
        eUI = 2, //ui
        eFile = 3, //文件
        eBuffer = 4, //缓存
    }

    public enum OutputLevel
    {
        eNone,
        eLog = 1,
        eLogWaning = 2,
        eLogError = 3,
    }

    //控制是否输出日志，总开关
    public static bool IsOpenLog
    {
        get;
        set;
    }

    private static Dictionary<string, bool> outputMouleDic = new Dictionary<string, bool>(); //输出模块dic
    private static Dictionary<OutputTarget, bool> outputTargetDic = new Dictionary<OutputTarget, bool>(); //输入目标dic
    private static Dictionary<OutputLevel, bool> outputLevelDic = new Dictionary<OutputLevel, bool>(); //输出等级dic
    private static Dictionary<string, bool> outputBufferDic = new Dictionary<string, bool>(); //输出特定模块的日志到缓存中的标志
    private const int UI_MAX_LOG_NUM = 1000; //ui显示日志的最大数量
    private const int LOCAL_FILE_MAX_NUM = 5; //本地最大缓存文件数量
    private static List<string> uiLogList = new List<string>(); //ui日志列表
    private static FileStream fs; 
    private static StreamWriter sw;
    private static LoopScrollRect wrapContent; //ui对象的组件
    private static string SERVER_URL = ""; //日志服务器地址
    private static Dictionary<string, List<string>> logBufferMap = new Dictionary<string, List<string>>(); //缓存特定系统的日志


    public static void Init(LoopScrollRect content = null)
    {
        ClearLocalFile();
        Application.logMessageReceived += LogCallback;

        //获取文件路径
        fs = File.Open(GetFilePath("QiPaiLogFile.txt"), FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        sw = new StreamWriter(fs);

        //获取ui组件
        wrapContent = content;

        //是否要输出日志
#if LOG_OPEN
        IsOpenLog = true;
#else
        IsOpenLog = false;
#endif
        
        //指定要输出的目标
        SwitchTarget(OutputTarget.eConsole, true);
        SwitchTarget(OutputTarget.eUI, true);
        SwitchTarget(OutputTarget.eBuffer, false);
#if LOG_TO_FILE
        SwitchTarget(OutputTarget.eFile, true);
#else
        SwitchTarget(OutputTarget.eFile, false);
#endif

        //指定输出的级别
        SwitchLevel(OutputLevel.eLog, true);
        SwitchLevel(OutputLevel.eLogWaning, true);
        SwitchLevel(OutputLevel.eLogError, true);
    }

    private static void LogCallback(string condition, string stackTrace, LogType type)
    {
        string result = "";
        if (type == LogType.Error || type == LogType.Log || type == LogType.Warning)
        {
            result = condition;
        }
        else
        {
            result = string.Format("{0}\n{1}", condition, stackTrace);
        }

        OutputUI(result);
        OutputFile(result);
    }

    [System.Diagnostics.Conditional("LOG_OPEN")]
    public static void Log(string type, string message)
    {
        if (!IsOuputLog(type, OutputLevel.eLog))
        {
            return;
        }

        message = ModifyLog(message, 0);
        OutputConsole(message, 0);
        OutputBuffer(type, message);
    }

    [System.Diagnostics.Conditional("LOG_OPEN")]
    public static void LogWarning(string type, string message)
    {
        if (!IsOuputLog(type, OutputLevel.eLogWaning))
        {
            return;
        }

        message = ModifyLog(message, 1);
        OutputConsole(message, 1);
        OutputBuffer(type, message);
    }

    [System.Diagnostics.Conditional("LOG_OPEN")]
    public static void LogError(string type, string message)
    {
        if (!IsOuputLog(type, OutputLevel.eLogError))
        {
            return;
        }

        message = ModifyLog(message, 2);
        OutputConsole(message, 2);
        OutputBuffer(type, message);
    }

    //开关日志输出模块
    public static void SwitchModule(string type, bool isOpen)
    {
        outputMouleDic[type] = isOpen;
    }

    //开关日志输出目标
    public static void SwitchTarget(OutputTarget target, bool isOpen)
    {
        outputTargetDic[target] = isOpen;
    }

    //开关日志输出等级
    public static void SwitchLevel(OutputLevel level, bool isOpen)
    {
        outputLevelDic[level] = isOpen;
    }

    //开启输出日志到buffer中
    public static void SwitchBuffer(string type, bool isOpen)
    {
        outputBufferDic[type] = isOpen;
    }

    //清理本地文件防止过多
    private static void ClearLocalFile()
    {
        string[] files = Directory.GetFiles(GetRootPath());
        int totalNum = files.Length;
        if (totalNum > LOCAL_FILE_MAX_NUM)
        {
            for(int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
                totalNum--;
                if(totalNum <= LOCAL_FILE_MAX_NUM)
                {
                    break;
                }
            }
        }
    }

    private static void OutputConsole(string message, int type)
    {
        if (!IsOutputTarget(OutputTarget.eConsole))
        {
            return;
        }

        if (type == 0)
        {
            Debug.Log(message);
        }
        else if (type == 1)
        {
            Debug.LogWarning(message);
        }
        else if (type == 2)
        {
            Debug.LogError(message);
        }
    }

    private static void OutputFile(string message)
    {
        if (!IsOutputTarget(OutputTarget.eFile))
        {
            return;
        }

        sw.WriteLine(message);
        sw.Flush();
    }

    private static void OutputUI(string message)
    {
        if (!IsOutputTarget(OutputTarget.eUI))
        {
            return;
        }

        if (uiLogList.Count >= UI_MAX_LOG_NUM)
        {
            uiLogList.Clear();
        }
        uiLogList.Add(message);
        if (wrapContent != null)
        {
            wrapContent.Init(uiLogList.Count, (obj, index) =>
            {
                //Text text = obj.GetComponent<Text>();
                //text.text = uiLogList[index];
            });
        }
    }

    private static void OutputBuffer(string type, string message)
    {
        //首先判断是否开启buffer的总开关
        if (!IsOutputTarget(OutputTarget.eBuffer))
        {
            return;
        }

        //判断是否开启特定模块输出到buffer的开关
        if (!IsOutputBuffer(type))
        {
            return;
        }

        List<string> logList = null;
        if (!logBufferMap.TryGetValue(type, out logList))
        {
            logList = new List<string>();
            logBufferMap[type] = logList;
        }

        if (logList != null)
        {
            logList.Add(message);
        }
    }


    private static string ModifyLog(string message, int type)
    {
        string result = "";
        //switch (type)
        //{
        //    case 0:
        //        {
        //            result += "common : ";
        //        }
        //        break;
        //    case 1:
        //        {
        //            result += "warning : ";
        //        }
        //        break;
        //    case 2:
        //        {
        //            result += "error : ";
        //        }
        //        break;
        //}
        result += message;
        return result;
    }

    private static string GetFilePath(string name)
    {
        string path = GetRootPath();
        string time = DateTime.Now.ToString("u").Replace(":", "-");
        return path + name + time;
    }

    private static string GetRootPath()
    {
        string dirPath = "";
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                {
                    dirPath = Application.persistentDataPath + "/Log/";
                }
                break;
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                {
                    dirPath = Application.dataPath + "/../Log/";
                }
                break;
        }

        if(!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        return dirPath;
    }

    //是否输入特定模块特定级别的日志
    private static bool IsOuputLog(string type, OutputLevel level)
    {
        bool rule1 = false;
        outputMouleDic.TryGetValue(type, out rule1);
        bool rule2 = false;
        outputLevelDic.TryGetValue(level, out rule2);
        bool rule3 = IsOpenLog;

        return rule1 && rule2 && rule3;
    }

    //是否输出到特定目标中
    private static bool IsOutputTarget(OutputTarget target)
    {
        bool result = false;
        outputTargetDic.TryGetValue(target, out result);
        return result;
    }

    //是否输出特定模块日志到缓存中
    private static bool IsOutputBuffer(string type)
    {
        bool result = false;
        outputBufferDic.TryGetValue(type, out result);
        return result;
    }

    //输出指定模块的内容到指定文件中
    public static void OutputBufferToFile(string module, string FileName)
    {
        List<string> logList = null;
        if (logBufferMap.TryGetValue(module, out logList))
        {
            if (logList != null)
            {
                string path = GetFilePath(FileName);
                FileStream fs = File.Open(path, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                for (int i = 0; i < logList.Count; i++)
                {
                    sw.WriteLine(logList[i]);
                }
                sw.Close();
                fs.Close();
            }
        }
    }

    public static List<string> GetBuffer(string type)
    {
        List<string> logList = null;
        logBufferMap.TryGetValue(type, out logList);
        return logList;
    }

    //清理buffer
    public static void ClearBuffer(string type)
    {
        List<string> logList = null;
        if (logBufferMap.TryGetValue(type, out logList))
        {
            if (logList != null)
            {
                logList.Clear();
            }
        }
    }

    public static void UploadFiles(string playerID)
    {
        if (string.IsNullOrEmpty(playerID))
        {
            Debuger.LogError("Log", "上传日志玩家id不能为空");
            return;
        }

        string[] files = Directory.GetFiles(GetRootPath());
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = Path.GetFileName(files[i]);
            byte[] fileContent = File.ReadAllBytes(files[i]);
            CoroutineUtility.Instance.StartCoroutine(CoUploadFile(playerID, fileName, fileContent));
        }
    }

    private static IEnumerator CoUploadFile(string playerID, string fileName, byte[] fileContent)
    {
        WWWForm wwwform = new WWWForm();
        wwwform.AddField("playerID", playerID);
        wwwform.AddBinaryData(fileName, fileContent);
        WWW www = new WWW(SERVER_URL, wwwform);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            Log("Log", string.Format("日志上传成功FileName:{0}", fileName));
        }
        else
        {
            LogError("Log", string.Format("日志上传错误FileName:{0} , Error:{1}", fileName, www.error));
        }
    }
}

