/*
 *  Log 过滤
 */

#if UNITY_DLL

using System;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

using UnityEngine;

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

    //[Conditional("USE_LOG")]
    /// <summary>
    /// common log
    /// </summary>
    /// <param name="log"></param>
    public static void d(string log)
    {
        if(LogLevel >= Level.Debug)
        {
            var s = "[debug] " + log;
            UnityEngine.Debug.Log(s);
            if(LogLevel >= Level.Net)
                BroadcastMessage(log);
        }
    }

    public static void d(string fmt, params object[] args)
    {
        if(LogLevel >= Level.Debug)
            d(string.Format(fmt, args));
    }
    public static void d(params object[] args)
    {
        if(LogLevel >= Level.Debug)
        {
            var msg = "";
            foreach(var i in args)
            {
                msg += string.Format("{0};", i);
            }
            d(msg);
        }
    }

    public static void w(string log)
    {
        if(LogLevel >= Level.Warning)
        {
            var s = "[warning] " + log;
            UnityEngine.Debug.LogWarning(s);
            if(LogLevel >= Level.Net)
                BroadcastMessage(s);
        }
    }

    public static void w(string fmt, params object[] args)
    {
        if(LogLevel >= Level.Warning)
            w(string.Format(fmt, args));
    }

    private static void e(string log)
    {
        if(LogLevel >= Level.Error)
        {
            var s = "[error] " + log;
            UnityEngine.Debug.LogError(s);
            if(LogLevel >= Level.Net)
                BroadcastMessage(log);
        }
    }

    public static void e(string fmt, params object[] args)
    {
        if(LogLevel >= Level.Error)
            e(string.Format(fmt, args));
    }
    public static void e(params object[] args)
    {
        if(LogLevel >= Level.Net)
        {
            var msg = "";
            foreach(var i in args)
            {
                msg += string.Format("{0};", i);
            }
            e(msg);
        }
    }

    public static void e(Exception ex)
    {
        if(LogLevel >= Level.Error)
            e(ex.ToString());
    }

}

#endif //UNITY_DLL