using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

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


/// <summary>
/// Assets/ABResources 下需要打包的资源目录配置
/// </summary>
[ExecuteInEditMode]
public class BundleConfig : ScriptableObject
{
    public const string ABResourceDir = "Assets/ABResources";
    public const string BundleConfigAssetPath = "Assets/BuildTools/BundleConfig.asset";
    [Serializable]
    public class DirCfg
    {
        public string dir = "";
        public bool include = false;
        public bool preDownload = false;
    }
    [HideInInspector, SerializeField]
    public DirCfg[] mDirs = new DirCfg[0];

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
    public static BundleConfig Instance()
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
#else
    public static BundleConfig Instance()
    {
        if(mInstance == null)
        {
            mInstance = AssetBundle.LoadFromFile(BundleConfigAssetPath" + ".bd").LoadAsset<BundleConfig>(BundleConfigAssetPath);
            if(mInstance == null)
            {
                mInstance = new BundleConfig();
            }
        }

        return mInstance;
    }

#endif
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
        foreach(var i in Directory.GetDirectories(BundleConfig.ABResourceDir, "*", SearchOption.TopDirectoryOnly))
        {
            bool include = false;
            bool preDownload = false;
            BundleConfig.DirCfg dir = null;
            var ui = UPath.go(i);
            if(mDirs.TryGetValue(ui, out dir) && dir != null)
            {
                include = dir.include;
                preDownload = dir.preDownload;
            }
            dirs[ui] = (new BundleConfig.DirCfg {
                dir = ui,
                include = include,
                preDownload = preDownload
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
        EditorGUILayout.LabelField("preDownload", guiOpts);
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
                mDirs[i.Key].preDownload = allPreDownloadTmp;
            }
        }
        EditorGUILayout.EndHorizontal();

        allInclude = mDirs.Where(i => i.Value.include).Count() == mBehavior.mDirs.Count();
        allPreDownload = mDirs.Where(i => i.Value.preDownload).Count() == mBehavior.mDirs.Count();

        foreach(var i in mDirs)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i.Key.Replace(BundleConfig.ABResourceDir + "/", "  "), guiOpts);
            mDirs[i.Key].include = EditorGUILayout.Toggle("", i.Value.include, guiOpts);
            mDirs[i.Key].preDownload = EditorGUILayout.Toggle("", i.Value.preDownload, guiOpts);
            EditorGUILayout.EndHorizontal();
        }

        var rect = EditorGUILayout.GetControlRect();
        if(GUI.Button(rect.Split(0, 4), "Refresh"))
        {
            Refresh();
        }
        if(GUI.Button(rect.Split(1, 4), "BAndroid"))
        {
            foreach(var i in mDirs.Where(ii => ii.Value.include))
            {
                BuildScript.BuildBundle(i.Key, i.Key.Replace(BundleConfig.ABResourceDir + "/", ""), BuildTarget.Android);
            }
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
}
#endif
