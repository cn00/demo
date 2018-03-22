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

public enum EABRoot
{
    PreDownload,
    Resources,
    UI,
    Level,
    Effect,
    Lua,
    Audio,
    Font,
}
public enum ESceneRoot
{
    Level,
}

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
/// Assets/ABResources 下需要打包的资源目录配置
/// </summary>
[ExecuteInEditMode]
public class BundleConfig : ScriptableObject
{
    public const string AssetBundleRoot = "AssetBundle/";
    public const string ABResRoot = "Assets/ABResources/";
    public const string BundleConfigAssetPath = ABResRoot + "common/config/BundleConfig.asset";

    public const string ImagesRegex = "(.png$|.jpg$|.tga$|.psd$|.tiff$|.gif$|.jpeg$)";
    public const string AudiosRegex = "(.mp3$|.ogg$|.wav$|.aiff$)";
    public const string VideosRegex = "(.mov$|.mpg$|.mp4$|.avi$|.asf$|.mpeg$)";
    public const string ObjectRegex = "(.asset$|.prefab$)";
    public const string TextRegex = "(.txt$|.lua$|.xml$|.yaml$|.bytes$)";
    public const string PunctuationRegex = "([!|\\@|\\#|\\$|\\%|\\^|\\&|\\*|\\(|\\)|\\-|\\+|\\=|\\[|\\]|\\{|\\}]|;|:|'|\"|,|<|\\.|>|\\?|/|\\\\| |\\t|\\r|\\n)";

    [Serializable]
    public class DirCfg
    {
        public string dir = "";
        public bool include = false;
        public bool rebuild = false;
    }
    [HideInInspector, SerializeField]
    public DirCfg[] mDirs = new DirCfg[0];

    public const string BundlePostfix = ".bd";
    public const string CompressedExtension = ".lzma";

    public List<string> ABResGroups
    {
        get { return mDirs.Select((i) => i.dir).ToList(); }
    }

    /// <summary>
    /// 需要打包的场景
    /// </summary>
    public List<string> ABSceneRoots = new List<string>
    {
        "Scene",
    };

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
            mInstance = AssetHelper.Instance.GetAssetSync<BundleConfig>(BundleConfigAssetPath);
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
    bool allPreDownload = false;
    BundleConfig mBehavior = null;
    Dictionary<string, BundleConfig.DirCfg> mDirs = new Dictionary<string, BundleConfig.DirCfg>();
    public void OnEnable()
    {
        mBehavior = (BundleConfig)target;
        Refresh();
    }

    void Refresh()
    {
        foreach(var i in mBehavior.mDirs)
        {
            mDirs[i.dir] = i;
        }
        var dirs = new Dictionary<string, BundleConfig.DirCfg>();
        foreach(var i in Directory.GetDirectories(BundleConfig.ABResRoot, "*", SearchOption.TopDirectoryOnly))
        {
            bool include = false;
            bool rebuild = false;
            BundleConfig.DirCfg dir = null;
            var ui = i.upath();
            if(mDirs.TryGetValue(ui, out dir) && dir != null)
            {
                include = dir.include;
                rebuild = dir.rebuild;
            }
            dirs[ui] = (new BundleConfig.DirCfg
            {
                dir = ui,
                include = include,
                rebuild = rebuild
            });
        }
        mDirs = dirs;
        mBehavior.mDirs = mDirs.Values.ToArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayoutOption[] guiOpts = new GUILayoutOption[]
        {
            GUILayout.Width(80),
            GUILayout.ExpandWidth(true),
        };

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", guiOpts);
        EditorGUILayout.LabelField("Include", guiOpts);
        EditorGUILayout.LabelField("Rebuild", guiOpts);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("All", guiOpts);
        var allIncludeTmp = EditorGUILayout.Toggle("", allInclude, guiOpts);
        if(allIncludeTmp != allInclude)
        {
            allInclude = allIncludeTmp;
            foreach(var i in mDirs)
            {
                mDirs[i.Key].include = allIncludeTmp;
            }
        }

        var allPreDownloadTmp = EditorGUILayout.Toggle("", allPreDownload, guiOpts);
        if(allPreDownloadTmp != allPreDownload)
        {
            allPreDownload = allPreDownloadTmp;
            foreach(var i in mDirs)
            {
                mDirs[i.Key].rebuild = allPreDownloadTmp;
            }
        }
        EditorGUILayout.EndHorizontal();

        allInclude = mDirs.Where(i => i.Value.include).Count() == mBehavior.mDirs.Count();
        allPreDownload = mDirs.Where(i => i.Value.rebuild).Count() == mBehavior.mDirs.Count();

        foreach(var i in mDirs)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i.Key.Replace(BundleConfig.ABResRoot, "  "), guiOpts);
            mDirs[i.Key].include = EditorGUILayout.Toggle("", i.Value.include, guiOpts);
            mDirs[i.Key].rebuild = EditorGUILayout.Toggle("", i.Value.rebuild, guiOpts);
            EditorGUILayout.EndHorizontal();
        }

        var rect = EditorGUILayout.GetControlRect();
        if(GUI.Button(rect.Split(0, 4), "Refresh"))
        {
            Refresh();
        }
        if(GUI.Button(rect.Split(1, 4), "BAndroid"))
        {
            Build();
        }
        //if(GUI.Button(rect.Split(2, 4), "Clean Build"))
        //{

        //}

        if(GUI.changed)
        {
            mBehavior.mDirs = mDirs.Values.ToArray();
            EditorUtility.SetDirty(mBehavior);
            //AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    void Build()
    {
        foreach(var i in mDirs.Where(ii => ii.Value.include))
        {
            BuildScript.BuildBundle(i.Key, i.Key.Replace(BundleConfig.ABResRoot, ""), BuildTarget.Android, i.Value.rebuild);
        }
    }
}
#endif
