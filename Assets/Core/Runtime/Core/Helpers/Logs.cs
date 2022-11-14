using System;
using UnityEngine;

namespace Plml
{
    public static class Logs
    {
        public static void Log(LogLevel lvl, string message)
        {
            switch (lvl)
            {
                case LogLevel.Info:
                    Info(message);
                    break;
                case LogLevel.Warning:
                    Warning(message);
                    break;
                case LogLevel.Error:
                    Error(message);
                    break;
            }
        }

        public static void Info(string message) => Debug.Log(message);
        public static void Warning(string message) => Debug.LogWarning(message);
        public static void Error(string message) => Debug.LogError(message);
    }
}