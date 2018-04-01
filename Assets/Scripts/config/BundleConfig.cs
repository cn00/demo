using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
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


/// <summary>
/// Assets/BundleRes 下需要打包的资源目录配置
/// </summary>
[ExecuteInEditMode]
public class BundleConfig : ScriptableObject
{
    public const string ManifestName = "manifest.yaml";
    public const string BundleResRoot = "Assets/BundleRes/";
    public const string BundleConfigAssetPath = BundleResRoot + "common/config/BundleConfig.asset";

    public const string ImagesRegex = "(.png$|.jpg$|.tga$|.psd$|.tiff$|.gif$|.jpeg$)";
    public const string AudiosRegex = "(.mp3$|.ogg$|.wav$|.aiff$)";
    public const string VideosRegex = "(.mov$|.mpg$|.mp4$|.avi$|.asf$|.mpeg$)";
    public const string ObjectRegex = "(.asset$|.prefab$)";
    public const string TextRegex = "(.txt$|.lua$|.xml$|.yaml$|.bytes$)";
    public const string PunctuationRegex = "(`|~|\\!|\\@|\\#|\\$|\\%|\\^|\\&|\\*|\\(|\\)|\\-|\\+|\\=|\\[|\\]|\\{|\\}]|;|:|'|\"|,|<|\\.|>|\\?|/|\\\\| |\\t|\\r|\\n)";

    public static string LocalManifestPath
    {
        get
        {
#if UNITY_EDITOR
            var version = ProjectConfig.Instance.Version.ToString();
            return AssetSys.CacheRoot + version + "/" + BundleConfig.ManifestName;
#else
            return AssetSys.CacheRoot + BundleConfig.ManifestName;
#endif
        }
    }

    [SerializeField]
    public AppVersion Version;

    [Serializable]
    public class BundleInfo
    {
        [SerializeField]
        string mName;
        public string Name { get { return mName; } set { mName = value; } }

        [SerializeField]
        string mModifyTime;
        public string ModifyTime { get { return mModifyTime; } set { mModifyTime = value; } }

        [SerializeField]
        string mBuildTime;
        public string BuildTime { get { return mBuildTime; } set { mBuildTime = value; } }

        [SerializeField]
        string mVersion;
        public string Version { get { return mVersion; } set { mVersion = value; } }
    }

    [Serializable]
    public class GroupInfo
    {
        [SerializeField]
        string mName;
        public string Name { get { return mName; } set { mName = value; } }

        public bool include = false;
        public bool rebuild = false;
        public bool fold = false;

        [SerializeField]
        List<BundleInfo> mBundles;
        public List<BundleInfo> Bundles { get { return mBundles; } set { mBundles = value; } }
    }

    [HideInInspector, SerializeField]
    GroupInfo[] mGroups;
    public GroupInfo[] Groups { get { return mGroups; } set { mGroups = value; } }

    public const string BundlePostfix = ".bd";
    public const string CompressedExtension = ".lzma";

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
#endif

    public static BundleConfig InstanceRuntime()
    {
        if(mInstance == null)
        {
            mInstance = AssetSys.Instance.GetAssetSync<BundleConfig>(BundleConfigAssetPath);
            if(mInstance == null)
            {
                mInstance = new BundleConfig();
            }
        }

        return mInstance;
    }

}

#if UNITY_EDITOR
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

[CustomEditor(typeof(BundleConfig))]
public class BundleConfigExtension : Editor
{
    bool allInclude = false;
    bool allRebuild = false;
    BundleConfig _Target = null;
    Dictionary<string, BundleConfig.GroupInfo> mGroupsDic = new Dictionary<string, BundleConfig.GroupInfo>();
    public void OnEnable()
    {
        _Target = (BundleConfig)target;
        RefreshGroups();
    }

    void RefreshGroups()
    {
        if(_Target.Groups != null)
        {
            // list to dictionary
            foreach(var i in _Target.Groups)
            {
                mGroupsDic[i.Name] = i;
            }
        }
        var newGroupsDic = new Dictionary<string, BundleConfig.GroupInfo>();
        foreach(var group in Directory.GetDirectories(BundleConfig.BundleResRoot, "*", SearchOption.TopDirectoryOnly))
        {
            bool include = false;
            bool rebuild = false;
            bool fold = false;
            BundleConfig.GroupInfo old = null;
            var name = group.upath().Replace(BundleConfig.BundleResRoot, "");
            if(mGroupsDic.TryGetValue(name, out old) && old != null)
            {
                include = old.include;
                rebuild = old.rebuild;
                fold = old.fold;
            }
            newGroupsDic[name] = (new BundleConfig.GroupInfo
            {
                Name = name,
                include = include,
                rebuild = rebuild,
                fold = fold
            });

            Dictionary<string, BundleConfig.BundleInfo> tempBundles = new Dictionary<string, BundleConfig.BundleInfo>();
            foreach(var bundle in Directory.GetDirectories(group, "*", SearchOption.TopDirectoryOnly))
            {
                var bundleInfo = tempBundles[bundle] = new BundleConfig.BundleInfo();
                bundleInfo.Name = bundle.upath().Replace(group + "/", "");
                long time = 0;
                foreach(var f in Directory.GetFiles(bundle, "*", SearchOption.AllDirectories).Where(i=>!i.EndsWith(".meta")))
                {
                    var finfo = new FileInfo(f);
                    var tutc = finfo.LastWriteTime.ToFileTime();
                    //AppLog.d("{0}:{1}", f, tutc);
                    if(time < tutc)
                        time = tutc;
                }
                bundleInfo.ModifyTime = time.ToString();
            }
            newGroupsDic[name].Bundles = tempBundles.Values.ToList();
        }
        mGroupsDic = newGroupsDic;
        _Target.Groups = mGroupsDic.Values.ToArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayoutOption[] guiOpts = new GUILayoutOption[]
        {
            GUILayout.Width(50),
            GUILayout.ExpandWidth(true),
        };

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Group", guiOpts);
        EditorGUILayout.LabelField("Include", guiOpts);
        EditorGUILayout.LabelField("Rebuild", guiOpts);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("All", guiOpts);
        var allIncludeTmp = EditorGUILayout.Toggle("", allInclude, guiOpts);
        if(allIncludeTmp != allInclude)
        {
            allInclude = allIncludeTmp;
            foreach(var i in mGroupsDic)
            {
                mGroupsDic[i.Key].include = allIncludeTmp;
            }
        }

        var allRebuildTmp = EditorGUILayout.Toggle("", allRebuild, guiOpts);
        if(allRebuildTmp != allRebuild)
        {
            allRebuild = allRebuildTmp;
            foreach(var i in mGroupsDic)
            {
                i.Value.rebuild = allRebuildTmp;
            }
        }
        EditorGUILayout.EndHorizontal();

        allInclude = mGroupsDic.Where(i => i.Value.include).Count() == _Target.Groups.Count();
        allRebuild = mGroupsDic.Where(i => i.Value.rebuild).Count() == _Target.Groups.Count();

        foreach(var i in mGroupsDic)
        {
            EditorGUILayout.BeginHorizontal();
            {
                //EditorGUILayout.LabelField(i.Key.Replace(BundleConfig.BundleResRoot, "  "), guiOpts);
                i.Value.fold = EditorGUILayout.Foldout(i.Value.fold, i.Key.Replace(BundleConfig.BundleResRoot, ""), true);
                i.Value.include = EditorGUILayout.Toggle("", i.Value.include, guiOpts);
                i.Value.rebuild = EditorGUILayout.Toggle("", i.Value.rebuild, guiOpts);
            }
            EditorGUILayout.EndHorizontal();
            if(i.Value.fold)
            {
                foreach(var f in i.Value.Bundles)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(f.Name.Replace(i.Key, "    "), guiOpts);
                        EditorGUILayout.LabelField(DateTime.FromFileTime(long.Parse(f.ModifyTime)).ToString("MM.dd HH:mm:ss"), guiOpts);
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

        if(GUI.changed)
        {
            _Target.Groups = mGroupsDic.Values.ToArray();
            EditorUtility.SetDirty(_Target);
            //AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    void Build(BuildTarget target)
    {
        foreach(var i in mGroupsDic.Where(ii => ii.Value.include))
        {
			BuildScript.BuildBundleGroup(BundleConfig.BundleResRoot + i.Key, target, i.Value.rebuild);
        }
        BuildScript.GenBundleManifest(target);
    }
}
#endif
