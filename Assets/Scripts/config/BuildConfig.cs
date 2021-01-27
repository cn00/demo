using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR
using BundleManifest = System.Collections.Generic.List<BuildConfig.GroupInfo>;

public static class BundleManifestExtension
{
    public static List<BundleInfo> AllBundles(this BundleManifest self)
    {
        return self.SelectMany(i => i.Bundles)
            .ToList();
    }
}

[Serializable]
public class AppVersion
{
    public uint Major = 0; // 主版本
    public uint Minor = 0; // 次版本
    public uint Patch = 0; // 补丁版本
    [NonSerialized] public bool Foldout = false;

    public uint BtnPerRow = 3;
    // public void MajorABtn(){++Major;}
    // public void MinorABtn(){++Minor;}
    // public void PatchABtn(){++Patch;}
    // public void MajorDBtn(){--Major;}
    // public void MinorDBtn(){--Minor;}
    // public void PatchDBtn(){--Patch;}


    public AppVersion()
    {
        Major = 1;
        Minor = 0;
        Patch = 0;
    }

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
        get { return new Version((int) Major, (int) Minor, (int) Patch); }
    }

    public static implicit operator AppVersion(Version v)
    {
        return new AppVersion((uint) v.Major, (uint) v.Minor, (uint) v.Build);
    }

    #if UNITY_EDITOR
    public void Draw(int indent, GUILayoutOption[] option = null)
    {
        // Name = EditorGUILayout.TextField("Major", Name);
        EditorGUILayout.BeginHorizontal();
        {
            Major = (uint) EditorGUILayout.IntField("Major", (int) Major);
            var rect = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect.Split(0, 4), "+"))
            {
                Major += 1;
            }

            if (GUI.Button(rect.Split(1, 4), "-"))
            {
                Major -= 1;
            }

            Major = (int) Major < 0 ? 0 : Major;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            Minor = (uint) EditorGUILayout.IntField("Minor", (int) Minor);
            var rect2 = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect2.Split(0, 4), "+"))
            {
                Minor += 1;
            }

            if (GUI.Button(rect2.Split(1, 4), "-"))
            {
                Minor -= 1;
            }

            Minor = (int) Minor < 0 ? 0 : Minor;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            Patch = (uint) EditorGUILayout.IntField("Patch", (int) Patch);
            var rect3 = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect3.Split(0, 4), "+"))
            {
                Patch += 1;
            }

            if (GUI.Button(rect3.Split(1, 4), "-"))
            {
                Patch -= 1;
            }

            Patch = (int) Patch < 0 ? 0 : Patch;
        }
        EditorGUILayout.EndHorizontal();
    }
    #endif
}

//[ExecuteInEditMode]
public class BuildConfig : SingletonAsset<BuildConfig>
{
    [Serializable]
    public class Config
    {
        public string Name = "name";
    
        public AppChannel Channel = AppChannel.and_bili;
    
        // public BuildTarget targetPlatform = BuildTarget.Android;
        public AppVersion version = new AppVersion("1.0.0");
    
        public uint BtnPerRow = 3;
        public bool BatchBuild = false;
        public bool AddTime = false;
        public string ProductName = "A3! 满开剧团";
        public string BundleId = "com.bili.a3";
        public string PackageName = "a3";
        public string BuildNum = "0";
        public List<string> DefineSymbols = new List<string>();
    
        #if UNITY_EDITOR
        public int BuildType = (int) AndroidBuildType.Debug;
        public AndroidBuildSystem AndroidBuildSystem = AndroidBuildSystem.Gradle;
        public iOSSdkVersion iOSSdkVersion = iOSSdkVersion.SimulatorSDK;
        public AppBuildOptions BuildOptionFlags = 0;
        #endif
    
        [NonSerialized] public bool Foldout = false;
    }
    
    public List<Config> Configs = new List<Config>();

    const string Tag = "BuildConfig";

    #region const

    public const string BundleResDir = "AppRes";
    public const string BundleResRoot = "Assets/" + BundleResDir + "/";

    public const string ManifestName = "manifest.yaml";
    public const string BundlePostfix = ".bd";
    public const string CompressedExtension = ""; //.lzma
    public const string LuaExtension = ".lua";

    #endregion const


    public static string LocalManifestPath
    {
        get { return AssetSys.CacheRoot + BuildConfig.ManifestName; }
    }

    public bool m_UseBundle = false;

    public bool UseBundle
    {
        get
        {
            #if UNITY_EDITOR
            return m_UseBundle;
            #else
            return true;
            #endif
        }
    }

    public AppLog.Level LogLevel = AppLog.Level.Debug;

    //runInBackground
    [HideInInspector, SerializeField] public bool runInBackground = false;

    [SerializeField, HideInInspector] public long LastBuildTime = 0L;

    [SerializeField, HideInInspector] public AppVersion Version = new AppVersion("1.0.0");

    [HideInInspector, SerializeField] public string Ip = "http://10.23.114.141:8008/";

    [HideInInspector, SerializeField] public string Port = "8008";

    /// <summary>
    /// http://ip:port/path/to/root/
    /// </summary>
    /// <value>The http root.</value>
    public string ServerRoot
    {
        get { return string.Format("http://{0}:{1}/", Ip, Port); }
    }

    [Serializable]
    public enum BuildAssetBundleOptions
    {
        /// <summary>
        ///   <para>Build assetBundle without any special option.</para>
        /// </summary>
        None = 0,

        /// <summary>
        ///   <para>Don't compress the data when creating the asset bundle.</para>
        /// </summary>
        UncompressedAssetBundle = 1,

        /// <summary>
        ///   <para>Includes all dependencies.</para>
        /// </summary>
        [Obsolete("This has been made obsolete. It is always enabled in the new AssetBundle build system introduced in 5.0.")]
        CollectDependencies = 2,

        /// <summary>
        ///   <para>Forces inclusion of the entire asset.</para>
        /// </summary>
        [Obsolete("This has been made obsolete. It is always enabled in the new AssetBundle build system introduced in 5.0.")]
        CompleteAssets = 4,

        /// <summary>
        ///   <para>Do not include type information within the AssetBundle.</para>
        /// </summary>
        DisableWriteTypeTree = 8,

        /// <summary>
        ///   <para>Builds an asset bundle using a hash for the id of the object stored in the asset bundle.</para>
        /// </summary>
        DeterministicAssetBundle = 16, // 0x00000010

        /// <summary>
        ///   <para>Force rebuild the assetBundles.</para>
        /// </summary>
        ForceRebuildAssetBundle = 32, // 0x00000020

        /// <summary>
        ///   <para>Ignore the type tree changes when doing the incremental build check.</para>
        /// </summary>
        IgnoreTypeTreeChanges = 64, // 0x00000040

        /// <summary>
        ///   <para>Append the hash to the assetBundle name.</para>
        /// </summary>
        AppendHashToAssetBundleName = 128, // 0x00000080

        /// <summary>
        ///   <para>Use chunk-based LZ4 compression when creating the AssetBundle.</para>
        /// </summary>
        ChunkBasedCompression = 256, // 0x00000100

        /// <summary>
        ///   <para>Do not allow the build to succeed if any errors are reporting during it.</para>
        /// </summary>
        StrictMode = 512, // 0x00000200

        /// <summary>
        ///   <para>Do a dry run build.</para>
        /// </summary>
        DryRunBuild = 1024, // 0x00000400

        PlaceHold_2048 = 2048,

        /// <summary>
        ///   <para>Disables Asset Bundle LoadAsset by file name.</para>
        /// </summary>
        DisableLoadAssetByFileName = 4096, // 0x00001000

        /// <summary>
        ///   <para>Disables Asset Bundle LoadAsset by file name with extension.</para>
        /// </summary>
        DisableLoadAssetByFileNameWithExtension = 8192, // 0x00002000
    }


    [Serializable]
    public class GroupInfo
    {
        public bool Foldout = false;
        public string Name;
        public bool mInclude = false;
        public bool mRebuild = false;

        [NonSerialized] public ulong mSize = 0ul;

        public ulong Size
        {
            protected set { mSize = value; }
            get
            {
                if (mSize == 0ul)
                    foreach (var i in Bundles)
                        mSize += i.Size;
                return mSize;
            }
        }

        [SerializeField] List<BundleInfo> mBundles;

        public List<BundleInfo> Bundles
        {
            get { return mBundles; }
            set { mBundles = value; }
        }

        public BundleInfo Find(string name)
        {
            return Bundles.Find(i => name == i.Name);
        }

        #if UNITY_EDITOR
        public void Refresh()
        {
            foreach (var i in mBundles)
            {
                if (i.Rebuild)
                {
                    UnityEngine.Debug.Log($"Refresh {i.Name}");
                    AssetDatabase.ImportAsset(BundleResRoot + i.Name.RReplace(".bd$", ""), ImportAssetOptions.ImportRecursive);
                }
            }
        }

        public void DrawGroup(int indent = 0, GUILayoutOption[] guiOpts = null)
        {
            EditorGUI.indentLevel += indent;
            EditorGUILayout.BeginHorizontal();
            {
                Foldout = EditorGUILayout.Foldout(Foldout, Name, true);
                if (Size < 1024) //B
                    EditorGUILayout.LabelField((Size).ToString(), guiOpts);
                else if (Size < 1024 * 1024) //K
                    EditorGUILayout.LabelField((Size / 1024).ToString() + "K", guiOpts);
                else
                    EditorGUILayout.LabelField((Size / 1024 / 1024).ToString() + "M", guiOpts);
                EditorGUILayout.LabelField("", guiOpts);

                var tmpRebuild = EditorGUILayout.Toggle(mRebuild);
                if (mRebuild != tmpRebuild)
                {
                    mRebuild = tmpRebuild;
                    foreach (var f in Bundles)
                    {
                        f.Rebuild = mRebuild;
                    }
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
                    // reimport
                    // var rect = EditorGUILayout.GetControlRect();
                    // GUIStyle style = new GUIStyle();
                    // style.alignment = TextAnchor.MiddleLeft;
                    // if(GUILayout.Button(f.Name.Replace(Name + "/", "- "), guiOpts))
                    // {
                    //     var path = BundleResRoot+f.Name.RReplace(".bd$", "");
                    //     AppLog.d(Tag, "reimport {0}", path);
                    //     AssetDatabase.ImportAsset(path, ImportAssetOptions.ImportRecursive);
                    // }
                    if (f.Size < 1024) //B
                        EditorGUILayout.LabelField((f.Size).ToString(), guiOpts);
                    else if (f.Size < 1024 * 1024) //K
                        EditorGUILayout.LabelField((f.Size / 1024).ToString() + "K", guiOpts);
                    else
                        EditorGUILayout.LabelField((f.Size / 1024 / 1024).ToString() + "M", guiOpts);

                    f.Rebuild = EditorGUILayout.Toggle(f.Rebuild);
                }
                EditorGUILayout.EndHorizontal();
            }

            mRebuild = !Bundles.Any(i => i.Rebuild == false);
        }
        #endif
    }

    [HideInInspector, NonSerialized] public bool ForceRebuild = false;

    [HideInInspector, SerializeField] public bool BuildScene = false;

    [HideInInspector, SerializeField] BundleManifest mGroups = new BundleManifest();

    public BundleManifest Groups
    {
        get { return mGroups; }
        set { mGroups = value; }
    }


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
        get
        {
            return Groups.Select((i) => i.Name)
                .ToList();
        }
    }

    #if UNITY_EDITOR
    public void RefreshGroups()
    {
        foreach (var i in mGroups)
        {
            i.Refresh();
        }
    }

    [HideInInspector, SerializeField] public BuildAssetBundleOptions BundleBuildOptions = (
        BuildAssetBundleOptions.None
        //   | BuildAssetBundleOptions.CompleteAssets
        |
        BuildAssetBundleOptions.ChunkBasedCompression |
        BuildAssetBundleOptions.DeterministicAssetBundle
        //   | BuildAssetBundleOptions.DryRunBuild
        //   | BuildAssetBundleOptions.AppendHashToAssetBundleName
    );

    public static void ClearLogs()
    {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }

    [SerializeField, HideInInspector] List<ChannelConfig> mChannels = null;

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

    [HideInInspector, SerializeField] public AssetBundleServer.Server BundleServer = new AssetBundleServer.Server();

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

            if (OutputDataReceived != null)
                process.OutputDataReceived += OutputDataReceived;
            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if (string.IsNullOrEmpty(e.Data))
                    return;
                if (e.Data.StartsWith(processingtag))
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
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("catch: " + e);
        }

        while (!finished)
        {
            if (total > 1)
            {
                EditorUtility.DisplayCancelableProgressBar("processing ...", info + ": " + processing + "/" + total, processing / total);
            }

            yield return null;
        }

        if (end != null)
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
        resourcePath.Dir()
            .CreateDir();
        File.Copy(BuildConfig.AssetPath, resourcePath, true);
        AssetDatabase.ImportAsset(resourcePath);


        var bundlePath = BuildConfig.AssetPath.Replace("Assets/", "Assets/AppRes/config/");
        bundlePath.Dir()
            .CreateDir();
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
            EditorUserBuildSettings.androidBuildType = (AndroidBuildType) config.BuildType;

            if (config.OptionFlags.HasFlag(AppBuildOptions.Development))
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        }
        else if (config.Channel.isIOS())
        {
            PlayerSettings.iOS.sdkVersion = config.iOSSdkVersion;
            PlayerSettings.iOS.buildNumber = config.BuildNum;
            EditorUserBuildSettings.iOSBuildConfigType = (iOSBuildType) config.BuildType;
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

        var EnvDefineSymbols = Environment.GetEnvironmentVariable("DefineSymbols");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(config.Channel.BuildTargetGroup()
            , EnvDefineSymbols + ";" + string.Join(";", config.DefineSymbols.ToArray()));

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
        var options = (UnityEditor.BuildOptions) config.OptionFlags;
        UnityEngine.Debug.Log($"{string.Join(", ", SCENES)}");
        BuildScript.GenericBuild(SCENES, config.OutputPath(), config.Channel.BuildTargetGroup(), config.Channel.BuildTarget(), options);
    }

    #endif
}