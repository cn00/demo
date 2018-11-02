using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Net;

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

/// <summary>
/// Assets/BundleRes 下需要打包的资源目录配置
/// </summary>
[ExecuteInEditMode]
public class BuildConfig : SingletonAsset<BuildConfig>
{
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
    string m_Ip = "http://10.23.114.141:8008/";

    [HideInInspector, SerializeField]
    string m_Port = "8008";

    /// <summary>
    /// http://ip:port/path/to/root/
    /// </summary>
    /// <value>The http root.</value>
    public string ServerRoot
    {
        get { return string.Format("http://{0}:{1}/", m_Ip, m_Port); }
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
    protected BundleManifest Groups { get { return mGroups; } set { mGroups = value; } }

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
    static string LocalIpAddress()
    {
        IPAddress ipAddress = NetSys.LocalIpAddress()[0];
        var strLocalIP = ipAddress.ToString();
        return strLocalIP;
    }

    public override bool Init()
    {
        return base.Init();
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
            Debug.LogError("workspace not in this config");
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
            Debug.LogError("Unknow ChannelConfig: " + config.Channel);
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

        Debug.Log("DefineSymbols: " + PlayerSettings.GetScriptingDefineSymbolsForGroup(config.Channel.BuildTargetGroup()));
    }

    public static void BuildPkg(ChannelConfig config)
    {
        ClearLogs();

        Active(config);

        config.BuildNum = (int.Parse(config.BuildNum) + 1).ToString();

        if (EditorUserBuildSettings.activeBuildTarget != config.Channel.BuildTarget())
        {
            Debug.LogError("workspace not in this config");
            return;
        }

        string[] SCENES = BuildScript.FindEnabledEditorScenes();
        var options = (UnityEditor.BuildOptions)config.OptionFlags;
        BuildScript.GenericBuild(SCENES, config.OutputPath(), config.Channel.BuildTargetGroup(), config.Channel.BuildTarget(), options);
    }

    #region CustomEditor
    [CustomEditor(typeof(BuildConfig))]
    public class Editor : UnityEditor.Editor
    {
        bool allInclude = false;
        bool allRebuild = false;
        bool showBundles = false;
        bool showBuilds = false;

        BuildConfig mTarget = null;
        public void OnEnable()
        {
            mTarget = target as BuildConfig;
        }

        void Refresh()
        {
            mTarget.m_Ip = LocalIpAddress();

            var newGroups = new BundleManifest();
            var groups = Directory.GetDirectories(BuildConfig.BundleResRoot, "*", SearchOption.TopDirectoryOnly);
            int n = 0;
            foreach (var group in groups)
            {
                EditorUtility.DisplayCancelableProgressBar("update group ...", group, (float)(++n) / groups.Length);

                var groupName = group.upath().Replace(BuildConfig.BundleResRoot, "");
                GroupInfo groupInfo = mTarget.Groups.Find(i => i.Name == groupName);
                if (groupInfo == null)
                {
                    groupInfo = new GroupInfo()
                    {
                        Name = groupName,
                        Bundles = new List<BundleInfo>(),
                    };
                }

                var newBundles = new List<BundleInfo>();
                foreach (var bundle in Directory.GetDirectories(group, "*", SearchOption.TopDirectoryOnly))
                {
                    var bundlePath = bundle.upath();
                    var bundleName = bundlePath.Replace(BundleResRoot, "") + BundlePostfix;
                    var assetBundle = AssetImporter.GetAtPath(bundlePath);
                    if (assetBundle != null)
                    {
                        assetBundle.assetBundleName = bundleName;
                    }

                    //var bundleName = bundle.upath().Replace(group + "/", "");
                    var bundleInfo = groupInfo.Bundles.Find(i => i.Name == bundleName);
                    if (bundleInfo == null)
                    {
                        bundleInfo = new BundleInfo()
                        {
                            Name = bundleName,
                        };
                    }

                    ulong time = 0;
                    foreach (var f in Directory.GetFiles(bundle, "*", SearchOption.AllDirectories).Where(i => !i.EndsWith(".meta")))
                    {
                        var assetImporter = AssetImporter.GetAtPath(f);
                        if (assetImporter != null)
                        {
                            assetImporter.assetBundleName = bundleName;//"";//
                            var assetTimeStamp = assetImporter.assetTimeStamp;
                            if (time < assetTimeStamp)
                                time = assetTimeStamp;
                        }
                    }

                    // if (time > 0)
                        newBundles.Add(bundleInfo);
                }//for 2

                //            AssetDatabase.GetAllAssetBundleNames();
                // AssetDatabase.RemoveUnusedAssetBundleNames();

                groupInfo.Bundles = newBundles;

                if (groupInfo.Bundles.Count > 0)
                    newGroups.Add(groupInfo);
            }//for 1
            mTarget.Groups = newGroups;

            EditorUtility.ClearProgressBar();
        }

        // if rename some folders, then should call this
        public static IEnumerator FixPrefabLuaPath()
        {
            var root = BuildConfig.BundleResRoot;
            var prefabPaths = Directory.GetFiles(root, "*.prefab", SearchOption.AllDirectories);
            var count = 0;
            foreach (var i in prefabPaths)
            {
                var p = i.upath();
                var prefab = AssetDatabase.LoadAssetAtPath(p, typeof(GameObject));
                var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                var luamonos = go.GetComponents<LuaMonoBehaviour>();
                foreach (var luamono in luamonos)
                {
                    if(luamono.luaScript == null || luamono.luaScript.Asset == null)
                    {
                        AppLog.w(p + " luamono.luaScript not set");
                        continue;
                    }
                    var tpath = AssetDatabase.GetAssetPath(luamono.luaScript.Asset);
                    tpath = tpath.Remove(tpath.Length - 4).Replace(BuildConfig.BundleResRoot, "");
                    if(luamono.luaScript.path != tpath)
                    {
                        luamono.luaScript.path = tpath; 
                        AppLog.d("FixPrefabLuaPath", p);
                        EditorUtility.SetDirty(prefab);
                    }
                }

                if(luamonos.Count() > 0)
                {
                    ++ count;
                    PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.Default);
                }
                MonoBehaviour.DestroyImmediate(go);
                EditorUtility.DisplayCancelableProgressBar("FixPrefabLuaPath ..."
                    , count + "/" + prefabPaths.Count() + i, (float)(count) / prefabPaths.Count());
                if(count % 10 == 0)
                    yield return null;
            }
            EditorUtility.ClearProgressBar();
        }
        void DrawBundleConfig(GUILayoutOption[] guiOpts)
        {
            // build
            GUILayout.Space(1f);
            {
                var rect = EditorGUILayout.GetControlRect();
                var rebuild = mTarget.ForceRebuild;
                var sn = 5;
                var idx = -1;
                if (GUI.Button(rect.Split(++idx, sn), "BuildWin"))
                {
                    BuildAB(BuildTarget.StandaloneWindows, rebuild);
                }
                if (GUI.Button(rect.Split(++idx, sn), "BuildAnd"))
                {
                    BuildAB(BuildTarget.Android, rebuild);
                }
                if (GUI.Button(rect.Split(++idx, sn), "BuildiOS"))
                {
                    BuildAB(BuildTarget.iOS, rebuild);
                }
                if (GUI.Button(rect.Split(++idx, sn), "BuildMac"))
                {
                    BuildAB(BuildTarget.StandaloneOSX, rebuild);
                }
                if (GUI.Button(rect.Split(++idx, sn), "Clean"))
                {
                }

            }

            // clean
            GUILayout.Space(1f);
            {
                var rect = EditorGUILayout.GetControlRect();
                var sn = 5;
                var idx = -1;
                if (GUI.Button(rect.Split(++idx, sn), "Refresh"))
                {
                    Refresh();
                }
                if (GUI.Button(rect.Split(++idx, sn), "CleanWin"))
                {
                    Directory.Delete(BuildScript.BundleOutDir + (BuildTarget.StandaloneWindows), true);
                }
                if (GUI.Button(rect.Split(++idx, sn), "CleanAnd"))
                {
                    Directory.Delete(BuildScript.BundleOutDir + (BuildTarget.Android), true);
                }
                if (GUI.Button(rect.Split(++idx, sn), "CleaniOS"))
                {
                    Directory.Delete(BuildScript.BundleOutDir + (BuildTarget.iOS), true);
                }
                if (GUI.Button(rect.Split(++idx, sn), "CleanMac"))
                {
                    Directory.Delete(BuildScript.BundleOutDir + (BuildTarget.iOS), true);
                }
            }

            // update prefab lua path
            GUILayout.Space(1f);
            {
                var rect = EditorGUILayout.GetControlRect();
                var sn = 4;
                var idx = -1;
                if (GUI.Button(rect.Split(++idx, sn), "FixPrefabLuaPath"))
                {
                    EditorCoroutine.StartCoroutine(FixPrefabLuaPath());
                }
            }


            EditorGUILayout.LabelField("LastBuildTime", DateTime.FromFileTime(mTarget.LastBuildTime).ToString("yyyy/MM/dd HH:mm:ss"));
            mTarget.ForceRebuild = EditorGUILayout.Toggle("ForceRebuild", mTarget.ForceRebuild, guiOpts);
            mTarget.BuildScene = EditorGUILayout.Toggle("BuildScene", mTarget.BuildScene, guiOpts);

            ++EditorGUI.indentLevel;
            foreach (var i in mTarget.Groups)
            {
                i.DrawGroup(0, guiOpts);
            }
            --EditorGUI.indentLevel;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            mTarget.runInBackground = PlayerSettings.runInBackground = EditorGUILayout.Toggle("runInBackground", mTarget.runInBackground);

            GUILayoutOption[] guiOpts = new GUILayoutOption[]
            {
                GUILayout.Width(30),
                GUILayout.ExpandWidth(true),
            };

            mTarget.Version.Draw(0, guiOpts);


            EditorGUILayout.Space();
            
            // Bundles
            showBundles = EditorGUILayout.Foldout(showBundles, "AssetBundle", true);
            if (showBundles)
            {
                EditorGUILayout.LabelField("HttpRoot");
                EditorGUILayout.BeginHorizontal();
                {
                    mTarget.m_Ip = EditorGUILayout.TextField(mTarget.m_Ip);
                    mTarget.m_Port = EditorGUILayout.TextField(mTarget.m_Port);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                using (var verticalScope2 = new EditorGUILayout.VerticalScope("box"))
                {
                    DrawBundleConfig(guiOpts);
                }
            }

            //apk ios.proj exe app etc.
            Inspector.DrawList("Channels", mTarget.Channels, ref showBuilds, false, item => {
                var i = item as ChannelConfig;
                if (string.IsNullOrEmpty(i.Name))
                {
                    i.Name = i.Channel + ":" + (int)i.Channel;
                }
            });

            // server
            Inspector.DrawComObj("BundleServer", mTarget.BundleServer);
            // if(mTarget.BundleServer.thread != null)
            //     mTarget.BundleServer.Runing = mTarget.BundleServer.thread.IsAlive;

            mTarget.DrawSaveButton();

            if (GUI.changed)
            {
                AppLog.LogLevel = mTarget.LogLevel;
                PlayerSettings.bundleVersion = mTarget.Version.ToString();
                EditorUtility.SetDirty(mTarget);
            }
        }

        void BuildAB(BuildTarget target, bool rebuild)
        {

            EditorCoroutine.StartCoroutine(BuildScript.BuildAssetBundle(target, rebuild, ()=>{
                if (mTarget.BuildScene)
                    BuildScript.BuildStreamingScene(target);

                BuildScript.GenBundleManifest(target);
                BuildScript.GenVersionFile(target);
                mTarget.Groups.Sort((a, b) => b.Size.CompareTo(a.Size));
            }));
        }

        private void OnDestroy()
        {
            //            AssetDatabase.SaveAssets();
        }
    }
    #endregion CustomEditor
#endif
}
