using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

//출시용 빌드를 위해 로그 제거
public static class Logger
{

    [Conditional("DEV_VER")]
    public static void Log(object message, Object context = null)
    {
        if (context != null)
            UnityEngine.Debug.Log($"[{System.DateTime.Now:HH:mm:ss.fff}] {message}", context);
        else
            UnityEngine.Debug.Log($"[{System.DateTime.Now:HH:mm:ss.fff}] {message}");
    }

    [Conditional("DEV_VER")]
    public static void LogWarning(object message, Object context = null)
    {
        if (context != null)
            UnityEngine.Debug.LogWarning($"[{System.DateTime.Now:HH:mm:ss.fff}] {message}", context);
        else
            UnityEngine.Debug.LogWarning($"[{System.DateTime.Now:HH:mm:ss.fff}] {message}");
    }

    public static void LogError(object message, Object context = null)
    {
        string logMsg = $"[{System.DateTime.Now:HH:mm:ss.fff}] {message}";
        
        if (context != null)
            UnityEngine.Debug.LogError(logMsg, context);
        else
            UnityEngine.Debug.LogError(logMsg);
    }
}
