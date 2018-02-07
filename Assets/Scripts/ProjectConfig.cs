﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class FGVersion
{
    public int Major = 0; // 主版本
    public int Minor = 0; // 次版本
    public int Patch = 0; // 补丁版本

    public FGVersion(string v)
    {
        var vs = v.Split('.');
        Major = int.Parse(vs[0]);
        Minor = int.Parse(vs[1]);
        if(vs.Length == 3)
            Patch = int.Parse(vs[2]);
    }
    public FGVersion(int major, int minor, int build)
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

    public static implicit operator FGVersion(Version v)
    {
        return new FGVersion(v.Major, v.Minor, v.Build);
    }

}


/// <summary>
/// ProjectConfig
/// </summary>
[ExecuteInEditMode]
public class ProjectConfig : ScriptableObject
{
    #region property
    public FGVersion Version;
    public bool UseBundle = false;
    public string LuaExtension = ".lua";
    #endregion property

    [Serializable]
    public class DirCfg
    {
        public string dir = "";
        public bool include = false;
        public bool preDownload = false;
    }
    [HideInInspector, SerializeField]
    public DirCfg[] mDirs = new DirCfg[0];

    static ProjectConfig mInstance = null;
#if UNITY_EDITOR
    [MenuItem("Tools/Create ProjectConfig.asset")]
    public static ProjectConfig Instance()
    {
        if(mInstance == null)
        {
            mInstance = AssetDatabase.LoadAssetAtPath<ProjectConfig>("Assets/ProjectConfig.asset");
            if(mInstance == null)
            {
                mInstance = new ProjectConfig();
                AssetDatabase.CreateAsset(mInstance, "Assets/ProjectConfig.asset");
            }
        }

        return mInstance;
    }
#else
    public static ProjectConfig Instance()
    {
        if(mInstance == null)
        {
            mInstance = AssetBundle.LoadFromFile("Assets/ProjectConfig.asset.bd").LoadAsset<ProjectConfig>("Assets/ProjectConfig.asset");
            if(mInstance == null)
            {
                mInstance = new ProjectConfig();
            }
        }

        return mInstance;
    }

#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(ProjectConfig))]
public class ProjectConfigExtension : Editor
{
    #region property
    public static bool allInclude = false;
    ProjectConfig mBehavior;
    #endregion property

    public void OnEnable()
    {
        mBehavior = (ProjectConfig)target;
        Refresh();
    }

    void Refresh()
    {
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayoutOption[] guiOpts = new GUILayoutOption[]
        {
            GUILayout.Width(80),
            GUILayout.ExpandWidth(true),
        };


        //if(GUI.Button(rect.Split(2, 4), "Clean Build"))
        //{

        //}

        if(GUI.changed)
        {
            EditorUtility.SetDirty(mBehavior);
            //AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif