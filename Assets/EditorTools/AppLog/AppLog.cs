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
    public class OnMessageReceivedHandler : EventArgs
    {
        public string Message;

        public OnMessageReceivedHandler(string msg)
        {
            Message = msg;
        }
    }

    public static string TAG = "[game]";

    public static int Port = 7788;

    public static bool isEditor = false;

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
        UnityEngine.Debug.Log( TAG + log);
        BroadcastMessage(log.ToString());
    }

    public static void d(string fmt, params object[] args)
    {
        d(string.Format(fmt, args));
    }
    public static void d(params object[] args)
    {
        var msg = "";
        foreach(var i in args)
        {
            msg += string.Format("{0};", i);
        }
        d(msg);
    }

    public static void w(string log)
    {
        UnityEngine.Debug.LogWarning(TAG + log);
        BroadcastMessage(log);
    }

    public static void w(string fmt, params object[] args)
    {
        w(string.Format(fmt, args));
    }

    private static void e(string log)
    {
        UnityEngine.Debug.LogErrorFormat("{0} {1}", TAG, log);
        BroadcastMessage("error: " + log);
    }

    public static void e(string fmt, params object[] args)
    {
        e(string.Format(fmt, args));
    }
    public static void e(params object[] args)
    {
        var msg = "";
        foreach(var i in args)
        {
            msg += string.Format("{0};", i);
        }
        e(msg);
    }

    public static void e(Exception ex)
    {
        e(ex.ToString());
    }

}

#endif //UNITY_DLL