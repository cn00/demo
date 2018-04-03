using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
static class RectExtension
{
    public static Rect Split(this Rect rect, int index, int count)
    {
        int r = (int)rect.width % count; // Remainder used to compensate width and position.
        int width = (int)(rect.width / count);
        rect.width = width + (index < r ? 1 : 0) + (index + 1 == count ? (rect.width - (int)rect.width) : 0f);
        if(index > 0)
        { rect.x += width * index + (r - (count - 1 - index)); }

        return rect;
    }
}
#endif

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
public class AppVersion
{
    [Range(0,9)]
    public int Major = 0; // 主版本
    [Range(0,9)]
    public int Minor = 0; // 次版本
    [Range(0,99)]
    public int Patch = 0; // 补丁版本

    public AppVersion(string v)
    {
        var vs = v.Split('.');
        Major = int.Parse(vs[0]);
        Minor = int.Parse(vs[1]);
        if(vs.Length > 2)
            Patch = int.Parse(vs[2]);
    }
    public AppVersion(int major, int minor, int build)
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
        get { return new Version(Major, Minor, Patch); }
    }

    public static implicit operator AppVersion(Version v)
    {
        return new AppVersion(v.Major, v.Minor, v.Build);
    }

}

/// <summary>
/// Assets/BundleRes 下需要打包的资源目录配置
/// </summary>
[ExecuteInEditMode]
public class BundleConfig : ScriptableObject
{
    #region const
    public const string ManifestName = "manifest.yaml";
    public const string BundleConfigAssetPath = "Assets/BundleRes/common/config/BundleConfig.asset";

    public const string ImagesRegex = "(.png$|.jpg$|.tga$|.psd$|.tiff$|.gif$|.jpeg$)";
    public const string AudiosRegex = "(.mp3$|.ogg$|.wav$|.aiff$)";
    public const string VideosRegex = "(.mov$|.mpg$|.mp4$|.avi$|.asf$|.mpeg$)";
    public const string ObjectRegex = "(.asset$|.prefab$)";
    public const string TextRegex = "(.txt$|.lua$|.xml$|.yaml$|.bytes$)";
    public const string PunctuationRegex = "(`|~|\\!|\\@|\\#|\\$|\\%|\\^|\\&|\\*|\\(|\\)|\\-|\\+|\\=|\\[|\\]|\\{|\\}]|;|:|'|\"|,|<|\\.|>|\\?|/|\\\\| |\\t|\\r|\\n)";

    public const string BundlePostfix = ".bd";
    public const string CompressedExtension = ".lzma";

    #endregion const

    #region static 

    #endregion static

    [SerializeField]
    string m_BundleResRoot = "Assets/BundleRes/";
    public static string BundleResRoot { get { return Instance().m_BundleResRoot; } }

    public string m_ServerRoot="http://10.23.114.141:8008/";
    /// <summary>
    /// http://ip:port/path/to/root/
    /// </summary>
    /// <value>The http root.</value>
    public string ServerRoot
    {
        get { return m_ServerRoot.EndsWith("/") ? m_ServerRoot : m_ServerRoot + "/"; }
    }

    public static string LocalManifestPath
    {
        get
        {
            return AssetSys.CacheRoot + BundleConfig.ManifestName;
        }
    }

    [SerializeField]
    public AppVersion Version;

    public bool UseBundle = false;
    public string LuaExtension = ".lua";

    [Serializable]
    public class BundleInfo : object
    {
        [SerializeField]
        string mName;
        [YamlDotNet.Serialization.YamlIgnore]
        public string Name { get { return mName; } set { mName = value; } }

        [SerializeField]
        string mModifyTime;
        [YamlDotNet.Serialization.YamlIgnore]
        public string ModifyTime { get { return mModifyTime; } set { mModifyTime = value; } }

        [SerializeField]
        [YamlDotNet.Serialization.YamlIgnore]
        string mBuildTime = "0";
        public string BuildTime { get { return mBuildTime; } set { mBuildTime = value; } }

        [SerializeField]
        string mVersion;
        public string Version { get { return mVersion; } set { mVersion = value; } }
    }

    [Serializable]
    public class GroupInfo : object
    {
        [SerializeField]
        string mName;
        public string Name { get { return mName; } set { mName = value; } }

        public bool include = false;
        public bool rebuild = false;
        public bool show = false;

        [SerializeField]
        List<BundleInfo> mBundles;
        public List<BundleInfo> Bundles { get { return mBundles; } set { mBundles = value; } }
    }

    [HideInInspector, SerializeField]
    List<GroupInfo> mGroups;
    public List<GroupInfo> Groups { get { return mGroups; } set { mGroups = value; } }

    public BundleInfo GetBundleInfo(string path)
    {
        var dirs = path.Split('/');
        GroupInfo group = null;
        foreach(var i in Groups)
        {
            if(i.Name == dirs[0])
            {
                group = i;
                break;
            }
        }
        if(group != null)
        {
            foreach(var j in group.Bundles)
            {
                if(j.Name == dirs[1])
                {
                    return j;
                }
            }
        }
        return null;
    }

    public List<string> ABResGroups
    {
        get { return Groups.Select((i) => i.Name).ToList(); }
    }

    static BundleConfig mInstance = null;
#if UNITY_EDITOR
    [MenuItem("Tools/Create BundleConfig.asset")]
#endif
    public static BundleConfig Instance()
    {
#if UNITY_EDITOR
        return InstanceEditor();
#else
        return InstanceRuntime();
#endif
    }

#if UNITY_EDITOR
    public static BundleConfig InstanceEditor()
    {
        if(mInstance == null)
        {
            mInstance = AssetDatabase.LoadAssetAtPath<BundleConfig>(BundleConfigAssetPath);
            if(mInstance == null)
            {
                mInstance = new BundleConfig();
                AssetDatabase.CreateAsset(mInstance, BundleConfigAssetPath);
            }
        }

        return mInstance;
    }
    public void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.WriteImportSettingsIfDirty(BundleConfigAssetPath);
        AssetDatabase.ImportAsset(BundleConfigAssetPath, ImportAssetOptions.DontDownloadFromCacheServer);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif

    public static BundleConfig InstanceRuntime()
    {
        if(mInstance == null)
        {
            var assetSubPath = BundleConfigAssetPath.Replace(BundleResRoot, "");
            var cachePath = AssetSys.CacheRoot + AssetSys.Instance.GetBundlePath(assetSubPath);
            if(File.Exists(cachePath))
            {
                mInstance = AssetSys.Instance.GetAssetSync<BundleConfig>(assetSubPath);
            }
            else
            {
                var respath = (assetSubPath.Replace(".asset", ""));
                mInstance = Resources.Load<BundleConfig>(respath);
            }

            if(mInstance == null)
            {
                mInstance = ScriptableObject.CreateInstance<BundleConfig>();
            }
        }

        return mInstance;
    }

    #region CustomEditor
#if UNITY_EDITOR

    [CustomEditor(typeof(BundleConfig))]
    public class BundleConfigExtension : Editor
    {
        bool allInclude = false;
        bool allRebuild = false;
        bool showBundles = true;
        public void OnEnable()
        {
            mInstance = (BundleConfig)target;
            RefreshGroups();
        }

        void RefreshGroups()
        {
            var newGroups = new List<GroupInfo>();
            foreach(var group in Directory.GetDirectories(BundleConfig.BundleResRoot, "*", SearchOption.TopDirectoryOnly))
            {
                var groupName = group.upath().Replace(BundleConfig.BundleResRoot, "");
                GroupInfo groupInfo = mInstance.Groups.Find(i => i.Name == groupName);
                if(groupInfo == null)
                {
                    groupInfo = new GroupInfo()
                    {
                        Name = groupName,
                        Bundles = new List<BundleInfo>(),
                    };
                }

                var newBundles = new List<BundleInfo>();
                foreach(var bundle in Directory.GetDirectories(group, "*", SearchOption.TopDirectoryOnly))
                {
                    var bundleName = bundle.upath().Replace(group + "/", "");
                    var bundleInfo = groupInfo.Bundles.Find(i => i.Name == bundleName);
                    if(bundleInfo == null)
                    {
                        bundleInfo = new BundleInfo()
                        {
                            Name = bundleName,
                        };
                    }

                    long time = 0;
                    foreach(var f in Directory.GetFiles(bundle, "*", SearchOption.AllDirectories).Where(i => !i.EndsWith(".meta")))
                    {
                        var finfo = new FileInfo(f);
                        var tutc = finfo.LastWriteTime.ToFileTime();
                        //AppLog.d("{0}:{1}", f, tutc);
                        if(time < tutc)
                            time = tutc;
                    }
                    bundleInfo.ModifyTime = time.ToString();

                    if(time > 0)
                        newBundles.Add(bundleInfo);
                }//for 2
                groupInfo.Bundles = newBundles;

                if(groupInfo.Bundles.Count > 0)
                    newGroups.Add(groupInfo);
            }//for 1
            mInstance.Groups = newGroups;
        }

        void DrawBundles(GUILayoutOption[] guiOpts)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Group", guiOpts);
                EditorGUILayout.LabelField("Include", guiOpts);
                EditorGUILayout.LabelField("Rebuild", guiOpts);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("All", guiOpts);
                var allIncludeTmp = EditorGUILayout.Toggle("", allInclude, guiOpts);
                if(allIncludeTmp != allInclude)
                {
                    allInclude = allIncludeTmp;
                    foreach(var i in mInstance.Groups)
                    {
                        i.include = allIncludeTmp;
                    }
                }

                var allRebuildTmp = EditorGUILayout.Toggle("", allRebuild, guiOpts);
                if(allRebuildTmp != allRebuild)
                {
                    allRebuild = allRebuildTmp;
                    foreach(var i in mInstance.Groups)
                    {
                        i.rebuild = allRebuildTmp;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            allInclude = mInstance.Groups.Where(i => i.include).Count() == mInstance.Groups.Count();
            allRebuild = mInstance.Groups.Where(i => i.rebuild).Count() == mInstance.Groups.Count();

            foreach(var i in mInstance.Groups)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    //EditorGUILayout.LabelField(i.Key.Replace(BundleConfig.BundleResRoot, "  "), guiOpts);
                    i.show = EditorGUILayout.Foldout(i.show, i.Name, true);
                    i.include = EditorGUILayout.Toggle("", i.include, guiOpts);
                    i.rebuild = EditorGUILayout.Toggle("", i.rebuild, guiOpts);
                }
                EditorGUILayout.EndHorizontal();

                if(i.show)
                {
                    foreach(var f in i.Bundles)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("  " + f.Name, guiOpts);
                            EditorGUILayout.LabelField(DateTime.FromFileTime(long.Parse(f.ModifyTime)).ToString("MM.dd HH:mm:ss"), guiOpts);
                            EditorGUILayout.LabelField(DateTime.FromFileTime(long.Parse(f.BuildTime)).ToString("MM.dd HH:mm:ss"), guiOpts);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            var rect = EditorGUILayout.GetControlRect();
            if(GUI.Button(rect.Split(0, 4), "Refresh"))
            {
                RefreshGroups();
            }
            if(GUI.Button(rect.Split(1, 4), "BuildAnd"))
            {
                Build(BuildTarget.Android);
            }
            if(GUI.Button(rect.Split(2, 4), "BuildiOS"))
            {
                Build(BuildTarget.iOS);
            }
            //if(GUI.Button(rect.Split(2, 4), "Clean Build"))
            //{

            //}
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayoutOption[] guiOpts = new GUILayoutOption[]
            {
                GUILayout.Width(50),
                GUILayout.ExpandWidth(true),
            };

            EditorGUILayout.Space();

            showBundles = EditorGUILayout.Foldout(showBundles, "Bundles", true);
            if(showBundles)
            {
                DrawBundles(guiOpts);
            }

            if(GUI.changed)
            {
                PlayerSettings.bundleVersion = mInstance.Version.ToString();
                EditorUtility.SetDirty(mInstance);
                //AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        void Build(BuildTarget target)
        {
            foreach(var i in mInstance.Groups.Where(ii => ii.include))
            {
                BuildScript.BuildBundleGroup(BundleConfig.BundleResRoot + i.Name, target, i.rebuild);
            }
            BuildScript.GenBundleManifest(target);
        }
    }
#endif
    #endregion CustomEditor

}
