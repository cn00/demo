using System;
using UnityEngine;

public class AppLog
{
    public delegate void OnMessageReceivedHandler(string msg);

    public static Action<object, OnMessageReceivedHandler> OnMessageReceived;
    public static bool isEditor = true;
    public static Level LogLevel { get; set; }

    public static void d(string tag, params object[] msg)
    {
        UnityEngine.Debug.Log($"{tag}:{msg}");
    }

    public static void e(string tag, params object[] msg)
    {
        UnityEngine.Debug.LogError($"{tag}:{msg}");
    }
    public static void w(string tag, params object[] msg)
    {
        UnityEngine.Debug.LogWarning($"{tag}:{msg}");
    }
    public enum Level
    {
        Debug
    }
}