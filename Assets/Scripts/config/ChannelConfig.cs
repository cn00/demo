using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum AppBuildOptions
{
    Development = 1,
    PlaceHolder_2 = 2,
    AutoRunPlayer = 4,
    ShowBuiltPlayer = 8,
    BuildAdditionalStreamedScenes = 16,
    AcceptExternalModificationsToPlayer = 32,
    InstallInBuildFolder = 64,
    WebPlayerOfflineDeployment = 128,
    ConnectWithProfiler = 256,
    AllowDebugging = 512,
    SymlinkLibraries = 1024,
    UncompressedAssetBundle = 2048,
    ConnectToHost = 4096,
    PlaceHolder_8192 = 8192,
    EnableHeadlessMode = 16384,
    BuildScriptsOnly = 32768,
    Il2CPP = 65536,
    ForceEnableAssertions = 131072,
    CompressWithLz4 = 262144,
    ForceOptimizeScriptCompilation = 524288,
    ComputeCRC = 1048576,
    StrictMode = 2097152
}

public enum AppChannel
{
    None,
    #region Android
    Android_begin,
    Android,
    Android_end,
    #endregion Android

    #region iOS
    iOS_begin,
    iOS,
    iOS_end
    #endregion iOS
}

public static class AppChannelExtension
{
    public static bool isIOS(this AppChannel self)
    {
        return self > AppChannel.iOS_begin && self < AppChannel.iOS_end;
    }

    public static bool isAndroid(this AppChannel self)
    {
        return self > AppChannel.Android_begin && self < AppChannel.Android_end;
    }
}

#if UNITY_EDITOR
[Serializable]
public class ChannelConfig : InspectorDraw
{
    public bool BatchBuild = false;
    public bool AddTime = false;
    public string ProductName = "A3! 满开剧团";
    public string BundleId = "com.bili.a3";
    public string PackageName = "a3";
    public string BuildNum = "0";
    public string DefineSymbols = "";

#if UNITY_ANDROID
    public AndroidBuildType AndroidBuildType = AndroidBuildType.Debug;
    public AndroidBuildSystem AndroidBuildSystem = AndroidBuildSystem.Internal;
#else
    public int AndroidBuildType = 0;
    public int AndroidBuildSystem = 0;
#endif

    public AppBuildOptions OptionFlags = 0;
    public AppChannel Channel = AppChannel.Android;

    public bool iosSim = false;

    public ChannelConfig copy()
    {
        var o = this;
        return new ChannelConfig()
        {
            ProductName = o.ProductName + "-copy",
            BundleId = o.BundleId,
            PackageName = o.PackageName,
            BuildNum = o.BuildNum,
            DefineSymbols = o.DefineSymbols,
            AndroidBuildType = o.AndroidBuildType,
            AndroidBuildSystem = o.AndroidBuildSystem,
            OptionFlags = o.OptionFlags,
            Channel = o.Channel,
        };
    }

    public override void DrawInspector(int indent = 0, GUILayoutOption[] guiOpts = null)
    {
        Name = Channel.ToString();
        base.DrawInspector(indent, guiOpts);
        EditorGUILayout.BeginHorizontal();
        {
            var rect = EditorGUILayout.GetControlRect();
            int sc = 4;
            int idx = -1;
            if (GUI.Button(rect.Split(++idx, sc), "Build"))
            {
                Build(this);
            }
            if (GUI.Button(rect.Split(++idx, sc), "Active"))
            {
                
            }
            if (GUI.Button(rect.Split(++idx, sc), "Copy"))
            {
                var copy = this.copy();
                BuildConfig.Instance().Channels.Insert(BuildConfig.Instance().Channels.IndexOf(this) + 1, copy);
            }
            if (GUI.Button(rect.Split(++idx, sc), "Delete"))
            {
                BuildConfig.Instance().Channels.Remove(this);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    public void CopyAar()
    {

    }

    public void Build(ChannelConfig config)
    {
        config.BuildNum = (int.Parse(config.BuildNum) + 1).ToString();

        var targetGroup = BuildTargetGroup.Android;
        var buildTarget = BuildTarget.Android;
        var PackageName = config.PackageName;
        if (config.AddTime)
            PackageName += DateTime.Now.ToString("-yyyyMdHHmmss");
        string target_dir = "./bin/" + PackageName + "-" + config.BuildNum + ".apk";
        if (config.Channel.isAndroid())
        {

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                AppLog.e("workspace not in android env");
                return;
            }
            CopyAar();
#if UNITY_ANDROID
            PlayerSettings.Android.bundleVersionCode = int.Parse(config.BuildNum);
            EditorUserBuildSettings.androidBuildSystem = config.AndroidBuildSystem;
            EditorUserBuildSettings.androidBuildType = config.AndroidBuildType;
#endif
        }
        else if (config.Channel.isIOS())
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
            {
                AppLog.e("workspace not in iOS env");
                return;
            }
            targetGroup = BuildTargetGroup.iOS;
            buildTarget = BuildTarget.iOS;
            target_dir = "./ios.proj." + PackageName;
            if (config.iosSim)
            {
                PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
                target_dir += ".sim";
            }
            else
                PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
            PlayerSettings.iOS.buildNumber = config.BuildNum;
        }
        else
        {
            AppLog.e("Unknow ChannelConfig: " + config.Channel);
        }
        PlayerSettings.SetApplicationIdentifier(targetGroup, config.BundleId);
        PlayerSettings.productName = config.ProductName;

        var DefineSymbols = Environment.GetEnvironmentVariable("DefineSymbols");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup
            , "UNITY_LIBER;USE_LOCALIZE;SERVER_PRO;VERSION_1_10_0;"
            + DefineSymbols + ";"
            + config.DefineSymbols);

        string[] SCENES = BuildScript.FindEnabledEditorScenes();
        var options = (UnityEditor.BuildOptions)config.OptionFlags;
        BuildScript.GenericBuild(SCENES, target_dir, targetGroup, buildTarget, options);
    }
}
#endif
