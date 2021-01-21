/*
 *  Log 过滤
 */


using System;
using System.Text;
using System.Diagnostics;

using UnityEngine;

#if UNITY_DLL
public static class AppLog
{
    public enum Level
    {
        Error = 0,
        Warning = 1,
        Debug = 2,
        Info = 3,

        Net = 9,
    }

    public class OnMessageReceivedHandler : EventArgs
    {
        public string Message;

        public OnMessageReceivedHandler(string msg)
        {
            Message = msg;
        }
    }

    public static bool isEditor = false;

    public static Level LogLevel = Level.Debug;

    public static event EventHandler<OnMessageReceivedHandler> OnMessageReceived;

    static void BroadcastMessage(string msg)
    {
        if(OnMessageReceived != null)
            OnMessageReceived(null, new OnMessageReceivedHandler(msg));
    }
    
    static StringBuilder logs = new StringBuilder(10240);

    //[Conditional("USE_LOG")]
    /// <summary>
    /// common log
    /// </summary>
    /// <param name="log"></param>
    [Conditional("USE_LOG")]
    public static void d(string tag, string log)
    {
        if(LogLevel >= Level.Debug)
        {
            var s = "App:"+tag+": " + log;
            UnityEngine.Debug.Log(s);
            if(LogLevel >= Level.Net)
                BroadcastMessage(log);
        }
    }


    [Conditional("USE_LOG")]
    public static void d(string tag, params object[] args)
    {
        if(LogLevel >= Level.Debug)
        {
            logs.Clear();
            foreach(var i in args)
            {
                logs.Append(string.Format("{0};", i));
            }
            d(tag, logs.ToString());
        }
    }

    [Conditional("USE_LOG")]
    public static void w(string tag, string log)
    {
        if(LogLevel >= Level.Warning)
        {
            var s = "App:"+tag+": " + log;
            UnityEngine.Debug.LogWarning(s);
            if(LogLevel >= Level.Net)
                BroadcastMessage(s);
        }
    }

    [Conditional("USE_LOG")]
    public static void w(string tag, params object[] args)
    {
        if(LogLevel >= Level.Warning)
        {
            logs.Clear();
            foreach (var i in args)
            {
                logs.Append(string.Format("{0};", i));
            }
            w(tag, logs.ToString());
            
        }
    }

    // [Conditional("USE_LOG")]
    private static void e(string tag, string log)
    {
        if(LogLevel >= Level.Error)
        {
            var s = "App:"+tag+": " + log;
            UnityEngine.Debug.LogError(s);
            if(LogLevel >= Level.Net)
                BroadcastMessage(log);
        }
    }

    
    public static void e(string tag, params object[] args)
    {
        if(LogLevel >= Level.Net)
        {
            logs.Clear();
            foreach (var i in args)
            {
                logs.Append(string.Format("{0};", i));
            }

            e(tag, logs.ToString());
        }
    }

    public static void e(string tag, Exception ex)
    {
        if(LogLevel >= Level.Error)
            e(tag, ex.ToString());
    }

}
#endif