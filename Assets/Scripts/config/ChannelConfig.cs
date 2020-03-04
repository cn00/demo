using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEngine.Serialization;
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
    and_bili,
    and_komeo_google,
    and_komeo_mycard,
    Android_end,
    #endregion Android

    #region iOS
    ios_begin,
    ios_bili,
    ios_end
    #endregion iOS
}

public static class AppChannelExtension
{
    public static bool isIOS(this AppChannel self)
    {
        return self > AppChannel.ios_begin && self < AppChannel.ios_end;
    }

    public static bool isAndroid(this AppChannel self)
    {
        return self > AppChannel.Android_begin && self < AppChannel.Android_end;
    }
}

public static class AppChannelEditorExtension
{

#if UNITY_EDITOR
    public static UnityEditor.BuildTarget BuildTarget(this AppChannel self)
    {
        var target = UnityEditor.BuildTarget.Android;
        if (self > AppChannel.ios_begin
            && self < AppChannel.ios_end)
            target = UnityEditor.BuildTarget.iOS;
        return target;
    }

    public static UnityEditor.BuildTargetGroup BuildTargetGroup(this AppChannel self)
    {
        var target = UnityEditor.BuildTargetGroup.Android;
        if (self > AppChannel.ios_begin
            && self < AppChannel.ios_end)
            target = UnityEditor.BuildTargetGroup.iOS;
        return target;
    }

#endif

}

#if UNITY_EDITOR
namespace UnityEditor
{
[Serializable]
public class ChannelConfig
{
    #region properties
    public string Name = "ChannelConfig";
    public bool BatchBuild = false;
    public bool AddTime = false;
    public string ProductName = "A3! 满开剧团";
    public string BundleId = "com.bili.a3";
    public string PackageName = "a3";
    public string BuildNum = "0"; 
    public string Version = "1.0.0";
    public List<string> DefineSymbols = new List<string>();

    public bool USE_LOG = false;
    public bool USE_EMULATOR = false;

    public int BuildType = (int)AndroidBuildType.Debug;
    public AndroidBuildSystem BuildSystem = AndroidBuildSystem.Gradle;

    public iOSSdkVersion iOSSdkVersion = iOSSdkVersion.SimulatorSDK;

    public AppBuildOptions OptionFlags = 0;
    public AppChannel Channel = AppChannel.and_bili;

    [NonSerialized]
    public bool Foldout = false;

    #endregion properties


    public bool ToggleSymbol(string symbol, bool toggle)
    {
        if (toggle && !DefineSymbols.Contains(symbol))
        {
            // if (!DefineSymbols.EndsWith("\n"))
            //     DefineSymbols += "\n";
            DefineSymbols.Add(symbol);
        }
        else if (!toggle && DefineSymbols.Contains(symbol))
            DefineSymbols.Remove(symbol);

        return toggle;
    }

    public string OutputPath()
    {
        string target_path = "";
        if (Channel.isAndroid())
        {
            if (BuildSystem == AndroidBuildSystem.Gradle
            && (OptionFlags.HasFlag(AppBuildOptions.AcceptExternalModificationsToPlayer)))
            {
                target_path = "and.proj";
                // PlayerSettings.Android.GradleProjName = "android." +  PackageName;
            }
            else
            {
                var name = Name;
                if (AddTime)
                    name += DateTime.Now.ToString("-yyyyMdHHmmss");
                Directory.CreateDirectory("./bin");
                target_path = "bin/" + name + "-" + BuildNum;
                if (Channel.BuildTarget() == UnityEditor.BuildTarget.Android)
                    target_path += ".apk";
            }
        }
        else if (Channel.isIOS())
        {
            target_path = "ios.proj";
            if (this.iOSSdkVersion == iOSSdkVersion.SimulatorSDK)
            {
                PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
                target_path += ".sim";
            }
        }
        return target_path;
    }

    public void ActiveBtn()
    {
        BuildConfig.Active(this);
    }
    public void BuildBtn()
    {
        BuildConfig.BuildPkg(this);
    }
    public void CopyBtn()
    {
        var copy = this.copy();
        BuildConfig.Instance().Channels.Insert(BuildConfig.Instance().Channels.IndexOf(this) + 1, copy);
    }
    public void DeleteBtn()
    {
        BuildConfig.Instance().Channels.Remove(this);
    }

    public void PatchXcodeBtn()
    {
        
    }
    
    public ChannelConfig copy()
    {
        return (ChannelConfig) this.MemberwiseClone();
    }

    public void AfterDraw()
    {
        ToggleSymbol("ENABLE_EMULATER", USE_EMULATOR);
        ToggleSymbol("USE_LOG", USE_LOG);
    }

    public void CopyAar()
    {

    }
}
}
#endif
