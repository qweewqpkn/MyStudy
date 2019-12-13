using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class CLogger
    {
        public static string LogTag = "HotUpdate";

        public static void LogError(string msg)
        {
            Debuger.LogError(LogTag, msg);
        }

        public static void Log(string msg)
        {
            Debuger.Log(LogTag, msg);
        }

        public static void LogWarn(string msg)
        {
            Debuger.LogWarning(LogTag, msg);
        }
    }
}

