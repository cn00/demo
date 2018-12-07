using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Diagnostics;

using BundleManifest = System.Collections.Generic.List<BuildConfig.GroupInfo>;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

public static class BundleManifestExtension
{
    public static List<BuildConfig.BundleInfo> AllBundles(this BundleManifest self)
    {
        return self.SelectMany(i => i.Bundles).ToList();
    }
}

[Serializable]
public class AppVersion
{
    public string Name;
    public uint Major = 0; // 主版本
    public uint Minor = 0; // 次版本
    public uint Patch = 0; // 补丁版本

    public AppVersion(string v)
    {
        var vs = v.Split('.');
        Major = uint.Parse(vs[0]);
        Minor = uint.Parse(vs[1]);
        if (vs.Length > 2)
            Patch = uint.Parse(vs[2]);
    }
    public AppVersion(uint major, uint minor, uint build)
    {
        Major = major;
        Minor = minor;
        Patch = build;
    }

    public override string ToString()
    {
        return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
    }

    public Version V
    {
        get { return new Version((int)Major, (int)Minor, (int)Patch); }
    }

    public static implicit operator AppVersion(Version v)
    {
        return new AppVersion((uint)v.Major, (uint)v.Minor, (uint)v.Build);
    }

#if UNITY_EDITOR
    public void Draw(int indent, GUILayoutOption[] option = null)
    {
        // Name = EditorGUILayout.TextField("Major", Name);
        EditorGUILayout.BeginHorizontal();
        {
            Major = (uint)EditorGUILayout.IntField("Major", (int)Major);
            var rect = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect.Split(0, 4), "+"))
            {
                Major += 1;
            }
            if (GUI.Button(rect.Split(1, 4), "-"))
            {
                Major -= 1;
            }
            Major = (int)Major < 0 ? 0 : Major;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            Minor = (uint)EditorGUILayout.IntField("Minor", (int)Minor);
            var rect2 = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect2.Split(0, 4), "+"))
            {
                Minor += 1;
            }
            if (GUI.Button(rect2.Split(1, 4), "-"))
            {
                Minor -= 1;
            }
            Minor = (int)Minor < 0 ? 0 : Minor;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            Patch = (uint)EditorGUILayout.IntField("Patch", (int)Patch);
            var rect3 = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect3.Split(0, 4), "+"))
            {
                Patch += 1;
            }
            if (GUI.Button(rect3.Split(1, 4), "-"))
            {
                Patch -= 1;
            }
            Patch = (int)Patch < 0 ? 0 : Patch;
        }
        EditorGUILayout.EndHorizontal();
    }
#endif

}

[ExecuteInEditMode]
public partial class BuildConfig : SingletonAsset<BuildConfig>
{
    const string Tag = "BuildConfig";
    #region const
    public const string BundleResDir = "BundleRes";
    public const string BundleResRoot = "Assets/" + BundleResDir + "/";

    public const string ManifestName = "manifest.yaml";
    public const string BundlePostfix = ".bd";
    public const string CompressedExtension = ".lzma";
    public const string LuaExtension = ".lua";

    #endregion const

    #region  test dictionary draw
    [Serializable]
    public class TestC1
    {
        public int _int = 0;
        public string _string = "sss";

    }
    public Dictionary<string, TestC1> _testDic = new Dictionary<string, TestC1>(){
        {"test_key1", new TestC1(){_int = 1, _string = "string_1"}},
        {"test_key2", new TestC1(){_int = 1, _string = "string_2"}},
        {"test_key3", new TestC1(){_int = 1, _string = "string_3"}},
        {"test_key4", new TestC1(){_int = 1, _string = "string_4"}},
    };
    #endregion test

    public static string LocalManifestPath
    {
        get
        {
            return AssetSys.CacheRoot + BuildConfig.ManifestName;
        }
    }

    public bool UseBundle = false;

    public AppLog.Level LogLevel = AppLog.Level.Debug;

    //runInBackground
    [HideInInspector, SerializeField]
    public bool runInBackground = false;

    [SerializeField, HideInInspector]
    public long LastBuildTime = 0L;

    [SerializeField, HideInInspector]
    public AppVersion Version = new AppVersion("1.0.0") { Name = "Version" };

    [HideInInspector, SerializeField]
    public string Ip = "http://10.23.114.141:8008/";

    [HideInInspector, SerializeField]
    public string Port = "8008";

    /// <summary>
    /// http://ip:port/path/to/root/
    /// </summary>
    /// <value>The http root.</value>
    public string ServerRoot
    {
        get { return string.Format("http://{0}:{1}/", Ip, Port); }
    }

    [Serializable]
    public class BundleBuildConfig
    {
        [Flags]
        public enum BuildAssetBundleOptions
        {
            //
            // Summary:
            //     Build assetBundle without any special option.
            None = 0,
            //
            // Summary:
            //     Don't compress the data when creating the asset bundle.
            UncompressedAssetBundle = 1,
            //
            // Summary:
            //     Includes all dependencies.
            CollectDependencies = 2,
            //
            // Summary:
            //     Forces inclusion of the entire asset.
            CompleteAssets = 4,
            //
            // Summary:
            //     Do not include type information within the AssetBundle.
            DisableWriteTypeTree = 8,
            //
            // Summary:
            //     Builds an asset bundle using a hash for the id of the object stored in the asset
            //     bundle.
            DeterministicAssetBundle = 16,
            //
            // Summary:
            //     Force rebuild the assetBundles.
            ForceRebuildAssetBundle = 32,
            //
            // Summary:
            //     Ignore the type tree changes when doing the incremental build check.
            IgnoreTypeTreeChanges = 64,
            //
            // Summary:
            //     Append the hash to the assetBundle name.
            AppendHashToAssetBundleName = 128,
            //
            // Summary:
            //     Use chunk-based LZ4 compression when creating the AssetBundle.
            ChunkBasedCompression = 256,
            //
            // Summary:
            //     Do not allow the build to succeed if any errors are reporting during it.
            StrictMode = 512,
            //
            // Summary:
            //     Do a dry run build.
            DryRunBuild = 1024,

            PlaceHolder2048 = 2048,
            //
            // Summary:
            //     Disables Asset Bundle LoadAsset by file name.
            DisableLoadAssetByFileName = 4096,
            //
            // Summary:
            //     Disables Asset Bundle LoadAsset by file name with extension.
            DisableLoadAssetByFileNameWithExtension = 8192
        }
        public BuildAssetBundleOptions Options;
    }

    [Serializable]
    public class BundleInfo //: InspectorDraw
    {
        [SerializeField]
        string mName;
        public string Name { get { return mName; } set { mName = value; } }

        [SerializeField]
        public ulong mSize = 0u;
        public ulong Size { get { return mSize; } set { mSize = value; } }

        [SerializeField]
        string mMd5;
        public string Md5 { get { return mMd5; } set { mMd5 = value; } }

        [SerializeField]
        string mVersion;
        public string Version { get { return mVersion; } set { mVersion = value; } }
    }

    [Serializable]
    public class GroupInfo
    {
        public bool Foldout = false;
        public string Name;
        public bool mInclude = false;
        public bool mRebuild = false;

        [NonSerialized]
        public ulong mSize = 0ul;
        public ulong Size
        {
            protected set { mSize = value; }
            get
            {
                if(mSize == 0ul)
                    foreach (var i in Bundles)
                        mSize += i.Size;
                return mSize;
            }
        }

        [SerializeField]
        List<BundleInfo> mBundles;
        public List<BundleInfo> Bundles { get { return mBundles; } set { mBundles = value; } }

        public BundleInfo Find(string name) { return Bundles.Find(i => name == i.Name); }

#if UNITY_EDITOR
        public void DrawGroup(int indent = 0, GUILayoutOption[] guiOpts = null)
        {
            EditorGUI.indentLevel += indent;
            EditorGUILayout.BeginHorizontal();
            {
                Foldout = EditorGUILayout.Foldout(Foldout, Name, true);
                if (Size < 1024)//B
                    EditorGUILayout.LabelField((Size).ToString(), guiOpts);
                else if (Size < 1024 * 1024)//K
                    EditorGUILayout.LabelField((Size / 1024).ToString() + "K", guiOpts);
                else
                    EditorGUILayout.LabelField((Size / 1024 / 1024).ToString() + "M", guiOpts);
                EditorGUILayout.LabelField("", guiOpts);

                // reimport
                if(GUILayout.Button("reimport"))
                {
                    AssetDatabase.ImportAsset(BundleResRoot+Name, ImportAssetOptions.ImportRecursive);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (Foldout)
            {
                DrawBundles(indent, guiOpts);
            }
            EditorGUI.indentLevel -= indent;
        }

        public void DrawBundles(int indent, GUILayoutOption[] guiOpts)
        {
            Bundles.Sort((i, j) => j.Size.CompareTo(i.Size));
            foreach (var f in Bundles)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(f.Name.Replace(Name + "/", "- "), guiOpts);
                    if (f.Size < 1024)//B
                        EditorGUILayout.LabelField((f.Size).ToString(), guiOpts);
                    else if (f.Size < 1024 * 1024)//K
                        EditorGUILayout.LabelField((f.Size / 1024).ToString() + "K", guiOpts);
                    else
                        EditorGUILayout.LabelField((f.Size / 1024 / 1024).ToString() + "M", guiOpts);
                    // reimport
                    if(GUILayout.Button("reimport"))
                    {
                        var path = BundleResRoot+f.Name.RReplace(".bd$", "");
                        AppLog.d(Tag, "reimport {0}", path);
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ImportRecursive);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
#endif
    }

    [HideInInspector]
    public bool ForceRebuild = false;

    [HideInInspector, SerializeField]
    public bool BuildScene = false;

    [HideInInspector, SerializeField]
    BundleManifest mGroups = new BundleManifest();
    public BundleManifest Groups { get { return mGroups; } protected set { mGroups = value; } }

    public BundleInfo GetBundleInfo(string path)
    {
        var dirs = path.Split('/');
        var group = Groups.Find(i => i.Name == dirs[0]);
        if (group != null && dirs.Length > 1)
        {
            var bundleName = dirs[0] + "/" + dirs[1];
            var bundle = group.Bundles.Find(i => i.Name == bundleName);
            return bundle;
        }
        return null;
    }

    public List<BundleInfo> AllBundles
    {
        // get {return (from l in Groups from i in l.Bundles select i).ToList(); }
        get { return Groups.AllBundles(); }
        // get { return Groups.Select(i => i.Bundles).SelectMany(j => j).ToList(); }
        // get {return Groups.Select(i => i.Bundles).SelectMany(j => j.Select(k => k.Name)).ToList(); }
    }

    public List<string> ABResGroups
    {
        get { return Groups.Select((i) => i.Name).ToList(); }
    }

#if UNITY_EDITOR
    [HideInInspector, SerializeField]
    public UnityEditor.BuildAssetBundleOptions BundleBuildOptions = (
                BuildAssetBundleOptions.None
            //   | BuildAssetBundleOptions.CompleteAssets
              | BuildAssetBundleOptions.ChunkBasedCompression
              | BuildAssetBundleOptions.DeterministicAssetBundle
            //   | BuildAssetBundleOptions.DryRunBuild
            //   | BuildAssetBundleOptions.AppendHashToAssetBundleName
            );

    public static void ClearLogs()
    {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }

    [SerializeField, HideInInspector]
    List<ChannelConfig> mChannels = null;
    public List<ChannelConfig> Channels
    {
        get { return mChannels == null ? (mChannels = new List<ChannelConfig>()) : mChannels; }
    }

    [MenuItem("Tools/Create/BundleConfig.asset")]
    public static BuildConfig Create()
    {
        mInstance = null;
        return Instance();
    }

    [HideInInspector, SerializeField]
    public AssetBundleServer.Server BundleServer = new AssetBundleServer.Server();
    public static string LocalIpAddress()
    {
        IPAddress ipAddress = NetSys.LocalIpAddress()[0];
        var strLocalIP = ipAddress.ToString();
        return strLocalIP;
    }

    public static IEnumerator Execute(string exe, string prmt 
        , DataReceivedEventHandler OutputDataReceived = null
        , Action end = null
        , float total = 0, string processingtag = "bash", string info = ""
    )
    {
        bool finished = false;
        var process = new System.Diagnostics.Process();
        var processing = 0f;
        try
        {
            // UnityEngine.Debug.Log(exe + " " + prmt);
            ProcessStartInfo pi = new ProcessStartInfo(exe, prmt);
            pi.WorkingDirectory = ".";
            pi.RedirectStandardInput = false;
            pi.RedirectStandardOutput = true;
            pi.RedirectStandardError = true;
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;

            if(OutputDataReceived != null)
                process.OutputDataReceived += OutputDataReceived;
            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if (string.IsNullOrEmpty(e.Data))
                    return;
                if(e.Data.StartsWith(processingtag))
                    ++processing;
                // UnityEngine.Debug.Log(e.Data);
            };
            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    UnityEngine.Debug.LogError(e.GetType() + ": " + e.Data);
            };
            process.Exited += (object sender, EventArgs e) =>
            {
                finished = true;
                UnityEngine.Debug.Log("Exit");
            };

            process.StartInfo = pi;
            process.EnableRaisingEvents = true;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            // process.WaitForExit();
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError("catch: " + e);
        }

        while (!finished)
        {
            if(total > 1)
            {
                EditorUtility.DisplayCancelableProgressBar("uploading ...", info + ": " + processing + "/" + total, processing/total);
            }
            yield return null;
        }
        if(end != null)
            end();

        // UnityEngine.Debug.Log("finished: " + process.ExitCode);
        EditorUtility.ClearProgressBar();
        yield return null;
    }

    public override void Save()
    {
        base.Save();
        CopyBundleConfigAsset();
    }

    public static void CopyBundleConfigAsset()
    {
        var resourcePath = BuildConfig.AssetPath.Replace("Assets/", "Assets/Resources/");
        resourcePath.Dir().CreateDir();
        File.Copy(BuildConfig.AssetPath, resourcePath, true);
        AssetDatabase.ImportAsset(resourcePath);


        var bundlePath = BuildConfig.AssetPath.Replace("Assets/", "Assets/BundleRes/config/");
        bundlePath.Dir().CreateDir();
        File.Copy(BuildConfig.AssetPath, bundlePath, true);
        AssetDatabase.ImportAsset(bundlePath);
    }

    public static void Active(ChannelConfig config)
    {
        if (EditorUserBuildSettings.activeBuildTarget != config.Channel.BuildTarget())
        {
            AppLog.e(Tag, "workspace not in this config");
            return;
        }
        if (config.Channel.isAndroid())
        {
            // CopyAar(config);

            PlayerSettings.Android.bundleVersionCode = int.Parse(config.BuildNum);
            EditorUserBuildSettings.androidBuildSystem = config.BuildSystem;
            EditorUserBuildSettings.androidBuildType = (AndroidBuildType)config.BuildType;

            if (config.OptionFlags.HasFlag(AppBuildOptions.Development))
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        }
        else if (config.Channel.isIOS())
        {
            if (config.Emulator)
            {
                PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
            }
            else
            {
                PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
            }
            PlayerSettings.iOS.buildNumber = config.BuildNum;
            EditorUserBuildSettings.iOSBuildConfigType = (iOSBuildType)config.BuildType;
            EditorUserBuildSettings.symlinkLibraries = true;
        }
        else
        {
            AppLog.e(Tag, "Unknow ChannelConfig: " + config.Channel);
        }
        PlayerSettings.SetApplicationIdentifier(config.Channel.BuildTargetGroup(), config.BundleId);
        PlayerSettings.productName = config.ProductName;
        PlayerSettings.bundleVersion = config.Version;

        if (config.OptionFlags.HasFlag(AppBuildOptions.Il2CPP))
        {
            PlayerSettings.SetScriptingBackend(config.Channel.BuildTargetGroup(), ScriptingImplementation.IL2CPP);
        }
        else
        {
            PlayerSettings.SetScriptingBackend(config.Channel.BuildTargetGroup(), ScriptingImplementation.Mono2x);
        }

        // var activeTarget = UnityEditor.EditorUserBuildSettings.SwitchActiveBuildTarget(config.Channel.BuildTargetGroup(), config.Channel.BuildTarget());

        var DefineSymbols = Environment.GetEnvironmentVariable("DefineSymbols");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(config.Channel.BuildTargetGroup()
            , ""
            + DefineSymbols + ";"
            + config.DefineSymbols);

        AssetDatabase.Refresh();

        AppLog.d(Tag, "DefineSymbols: " + PlayerSettings.GetScriptingDefineSymbolsForGroup(config.Channel.BuildTargetGroup()));
    }

    public static void BuildPkg(ChannelConfig config)
    {
        ClearLogs();

        Active(config);

        config.BuildNum = (int.Parse(config.BuildNum) + 1).ToString();

        if (EditorUserBuildSettings.activeBuildTarget != config.Channel.BuildTarget())
        {
            AppLog.e(Tag, "workspace not in this config");
            return;
        }

        string[] SCENES = BuildScript.FindEnabledEditorScenes();
        var options = (UnityEditor.BuildOptions)config.OptionFlags;
        BuildScript.GenericBuild(SCENES, config.OutputPath(), config.Channel.BuildTargetGroup(), config.Channel.BuildTarget(), options);
    }

#endif
}
