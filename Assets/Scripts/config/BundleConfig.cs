using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Net;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

public static class PathExtension
{
    public static string upath(this string self)
    {
        return self.Trim()
            .Replace("\\", "/")
            .Replace("//", "/");
    }
    public static string wpath(this string self)
    {
        return self.Trim()
            .Replace("/", "\\")
            .Replace("\\\\", "\\");
    }

    public static bool IsImage(this string self)
    {
        var matches = Regex.Matches(self, BundleConfig.ImagesRegex);
        return matches.Count > 0;
    }
    public static bool IsAudio(this string self)
    {
        var matches = Regex.Matches(self, BundleConfig.AudiosRegex);
        return matches.Count > 0;
    }
    public static bool IsVideo(this string self)
    {
        var matches = Regex.Matches(self, BundleConfig.VideosRegex);
        return matches.Count > 0;
    }
    public static bool IsObject(this string self)
    {
        var matches = Regex.Matches(self, BundleConfig.ObjectRegex);
        return matches.Count > 0;
    }
    public static bool IsText(this string self)
    {
        var matches = Regex.Matches(self, BundleConfig.TextRegex);
        return matches.Count > 0;
    }
}


[Serializable]
public class AppVersion : InspectorDraw
{
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
    public override void DrawInspector(int name, GUILayoutOption[] option = null)
    {
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
public class BundleConfig : SingletonAsset<BundleConfig>
{
    #region const
    public const string BundleResDir = "BundleRes";
    public const string BundleResRoot = "Assets/" + BundleResDir + "/";
    public const string BundleConfigAssetPath = BundleResRoot + BundleConfigAssetSubPath;

    public const string ManifestName = "manifest.yaml";
    public const string BundleConfigAssetSubPath = "common/config/BundleConfig.asset";

    public const string ImagesRegex = "(.png$|.jpg$|.tga$|.psd$|.tiff$|.gif$|.jpeg$)";
    public const string AudiosRegex = "(.mp3$|.ogg$|.wav$|.aiff$)";
    public const string VideosRegex = "(.mov$|.mpg$|.mp4$|.avi$|.asf$|.mpeg$)";
    public const string ObjectRegex = "(.asset$|.prefab$)";
    public const string TextRegex = "(.txt$|.lua$|.xml$|.yaml$|.sql$|.bytes$)";
    public const string PunctuationRegex = "(`|~|\\!|\\@|\\#|\\$|\\%|\\^|\\&|\\*|\\(|\\)|\\-|\\+|\\=|\\[|\\]|\\{|\\}]|;|:|'|\"|,|<|\\.|>|\\?|/|\\\\| |\\t|\\r|\\n)";

    public const string BundlePostfix = ".bd";
    public const string CompressedExtension = ".lzma";
    public const string LuaExtension = ".lua";

    #endregion const

    #region static 

    #endregion static

    public static string LocalManifestPath
    {
        get
        {
            return AssetSys.CacheRoot + BundleConfig.ManifestName;
        }
    }

    public bool UseBundle = false;

    [SerializeField, HideInInspector]
    public long LastBuildTime = 0L;

    [SerializeField, HideInInspector]
    public AppVersion Version = new AppVersion("1.0.0") { Name = "Version" };

    [HideInInspector, SerializeField]
    string m_ServerRoot = "http://10.23.114.141:8008/";
    /// <summary>
    /// http://ip:port/path/to/root/
    /// </summary>
    /// <value>The http root.</value>
    public string ServerRoot
    {
        set { m_ServerRoot = value; }
        get { return m_ServerRoot.EndsWith("/") ? m_ServerRoot : m_ServerRoot + "/"; }
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
    public class GroupInfo : InspectorDraw
    {
        public bool mInclude = false;
        public bool mRebuild = false;

        public ulong mSize = 0ul;
        public ulong Size
        {
            get
            {
                var sum = 0ul;
                foreach (var i in Bundles)
                    sum += i.Size;
                return sum;
            }
        }

        [SerializeField]
        List<BundleInfo> mBundles;
        public List<BundleInfo> Bundles { get { return mBundles; } set { mBundles = value; } }

        public BundleInfo Find(string name) { return Bundles.Find(i => name == i.Name); }

#if UNITY_EDITOR
        public override void Draw(int indent = 0, GUILayoutOption[] guiOpts = null)
        {
            EditorGUI.indentLevel += indent;
            EditorGUILayout.BeginHorizontal();
            {
                mFoldOut = EditorGUILayout.Foldout(mFoldOut, Name, true);
                if (Size < 1024)//K
                    EditorGUILayout.LabelField((Size).ToString(), guiOpts);
                else if (Size < 1024 * 1024)//K
                    EditorGUILayout.LabelField((Size / 1024).ToString() + "K", guiOpts);
                else
                    EditorGUILayout.LabelField((Size / 1024 / 1024).ToString() + "M", guiOpts);
                EditorGUILayout.LabelField("", guiOpts);
                //                mRebuild = EditorGUILayout.Toggle("", mRebuild, guiOpts);
            }
            EditorGUILayout.EndHorizontal();
            if (mFoldOut)
            {
                //                ++EditorGUI.indentLevel;
                DrawInspector(indent, guiOpts);
                //                --EditorGUI.indentLevel;
            }
            EditorGUI.indentLevel -= indent;
        }

        public override void DrawInspector(int indent, GUILayoutOption[] guiOpts)
        {
            Bundles.Sort((i, j) => j.Size.CompareTo(i.Size));
            foreach (var f in Bundles)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(f.Name, guiOpts);
                    if (f.Size < 1024)//K
                        EditorGUILayout.LabelField((f.Size).ToString(), guiOpts);
                    else if (f.Size < 1024 * 1024)//K
                        EditorGUILayout.LabelField((f.Size / 1024).ToString() + "K", guiOpts);
                    else
                        EditorGUILayout.LabelField((f.Size / 1024 / 1024).ToString() + "M", guiOpts);

                    // EditorGUILayout.LabelField(DateTime.FromFileTime(long.Parse(f.ModifyTime)).ToString("MM.dd HH:mm:ss"), guiOpts);
                    // EditorGUILayout.LabelField(DateTime.FromFileTime(long.Parse(f.BuildTime)).ToString("MM.dd HH:mm:ss"), guiOpts);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
#endif
    }

    [HideInInspector, SerializeField]
    public bool ForceRebuild = false;
    [HideInInspector, SerializeField]
    List<GroupInfo> mGroups = new List<GroupInfo>();
    public List<GroupInfo> Groups { get { return mGroups; } set { mGroups = value; } }

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
        get { return Groups.Select(i => i.Bundles).SelectMany(j => j).ToList(); }
        // get {return Groups.Select(i => i.Bundles).SelectMany(j => j.Select(k => k.Name)).ToList(); }
    }

    public List<string> ABResGroups
    {
        get { return Groups.Select((i) => i.Name).ToList(); }
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Create BundleConfig.asset")]
    public static BundleConfig Create()
    {
        mInstance = null;
        return Instance();
    }

    [HideInInspector, SerializeField]
    public AssetBundleServer.Server mServer = new AssetBundleServer.Server() { Name = "BundleServer" };
    void Refresh()
    {
        mInstance.m_ServerRoot = "http://" + LocalIpAddress() + ":8008/";

        var newGroups = new List<GroupInfo>();
        var groups = Directory.GetDirectories(BundleConfig.BundleResRoot, "*", SearchOption.TopDirectoryOnly);
        int n = 0;
        foreach (var group in groups)
        {
            EditorUtility.DisplayCancelableProgressBar("update group ...", group, (float)(++n) / groups.Length);

            var groupName = group.upath().Replace(BundleConfig.BundleResRoot, "");
            GroupInfo groupInfo = Groups.Find(i => i.Name == groupName);
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
                        assetImporter.assetBundleName = "";//bundleName;
                        var assetTimeStamp = assetImporter.assetTimeStamp;
                        if (time < assetTimeStamp)
                            time = assetTimeStamp;
                    }
                }

                if (time > 0)
                    newBundles.Add(bundleInfo);
            }//for 2

            //            AssetDatabase.GetAllAssetBundleNames();
            AssetDatabase.RemoveUnusedAssetBundleNames();

            groupInfo.Bundles = newBundles;

            if (groupInfo.Bundles.Count > 0)
                newGroups.Add(groupInfo);
        }//for 1
        Groups = newGroups;

        EditorUtility.ClearProgressBar();
    }

    static string LocalIpAddress()
    {
        var hostname = Dns.GetHostName();
        if (!hostname.EndsWith(".local"))
            hostname = hostname + ".local";

        IPHostEntry ipHost = Dns.GetHostEntry(hostname);
        string s = "";
        foreach (var i in ipHost.AddressList)
        {
            s += "=" + i.ToString();
        }
        AppLog.d("hostname={0}{1}", hostname, s);

        IPAddress ipAddress = ipHost.AddressList[0];
        var strLocalIP = ipAddress.ToString();
        return strLocalIP;
    }

    public override bool Init()
    {
        mInstance.Refresh();
        return base.Init();
    }

    public override void Save()
    {
        base.Save();
        CopyBundleConfigAsset();
    }

    public static void CopyBundleConfigAsset()
    {
        var configRes = BundleConfig.BundleConfigAssetPath.Replace(BundleResDir, "Resources");
        var configDir = Path.GetDirectoryName(configRes);
        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }
        File.Copy(BundleConfig.AssetPath, BundleConfig.BundleConfigAssetPath, true);
        File.Copy(BundleConfig.AssetPath, configRes, true);
        AssetDatabase.ImportAsset(configRes);
    }

#endif //UNITY_EDITOR

    #region CustomEditor
#if UNITY_EDITOR

    [CustomEditor(typeof(BundleConfig))]
    public class Editor : UnityEditor.Editor
    {
        bool allInclude = false;
        bool allRebuild = false;
        bool showBundles = true;
        public void OnEnable()
        {
            mInstance = Instance();
            //            mInstance.Refresh();
        }

        void DrawBundles(GUILayoutOption[] guiOpts)
        {
            foreach (var i in mInstance.Groups)
            {
                i.Draw(0, guiOpts);
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayoutOption[] guiOpts = new GUILayoutOption[]
            {
                GUILayout.Width(30),
                GUILayout.ExpandWidth(true),
            };

            mInstance.Version.Draw(0, guiOpts);

            EditorGUILayout.LabelField("LastBuildTime", DateTime.FromFileTime(mInstance.LastBuildTime).ToString("yyyy/MM/dd HH:mm:ss"));

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("HttpRoot");
                var rect = EditorGUILayout.GetControlRect();
                if (GUI.Button(rect.Split(0, 3), "Refresh"))
                {
                    mInstance.Refresh();
                }
            }
            EditorGUILayout.EndHorizontal();
            mInstance.m_ServerRoot = EditorGUILayout.TextField(mInstance.m_ServerRoot);

            EditorGUILayout.Space();

            using (var verticalScope = new EditorGUILayout.VerticalScope("box"))
            {
                ++EditorGUI.indentLevel;
                showBundles = EditorGUILayout.Foldout(showBundles, "Bundles", true);
                if (showBundles)
                {
                    mInstance.ForceRebuild = EditorGUILayout.Toggle("ForceRebuild", mInstance.ForceRebuild, guiOpts);

                    // build
                    GUILayout.Space(1f);
                    {
                        var rect = EditorGUILayout.GetControlRect();
                        var rebuild = mInstance.ForceRebuild;
                        var sn = 4;
                        var idx = -1;
                        if (GUI.Button(rect.Split(++idx, sn), "BuildWin"))
                        {
                            Build(BuildTarget.StandaloneWindows, rebuild);
                        }
                        if (GUI.Button(rect.Split(++idx, sn), "BuildAnd"))
                        {
                            Build(BuildTarget.Android, rebuild);
                        }
                        if (GUI.Button(rect.Split(++idx, sn), "BuildiOS"))
                        {
                            Build(BuildTarget.iOS, rebuild);
                        }
                        if (GUI.Button(rect.Split(++idx, sn), "BuildMac"))
                        {
                            Build(BuildTarget.StandaloneOSX, rebuild);
                        }
                    }

                    // clean
                    GUILayout.Space(1f);
                    {
                        var rect = EditorGUILayout.GetControlRect();
                        var sn = 4;
                        var idx = -1;
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

                    // Bundles
                    // using (var verticalScope2 = new EditorGUILayout.VerticalScope("box"))
                    {
                        DrawBundles(guiOpts);
                    }
                }
                --EditorGUI.indentLevel;
            }

            // server
            mInstance.mServer.Draw();

            if (GUI.changed)
            {
                PlayerSettings.bundleVersion = mInstance.Version.ToString();
                EditorUtility.SetDirty(mInstance);
            }
        }

        void Build(BuildTarget target, bool rebuild)
        {
            BuildScript.BuildAssetBundle(target, rebuild);

            BuildScript.BuildStreamingScene(target);

            BuildScript.GenBundleManifest(target);
            BuildScript.GenVersionFile(target);
        }

        private void OnDestroy()
        {
            //            AssetDatabase.SaveAssets();
        }
    }
#endif
    #endregion CustomEditor

}
