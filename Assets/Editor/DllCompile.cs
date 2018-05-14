using System.IO;
using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Net;
using System.Reflection;

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

[ExecuteInEditMode]
public class DllCompile  : ScriptableObject
{

    [Serializable]
    public class BundleInfo : object
    {
        [SerializeField]
        public string mName = ".dll";
        public string Name { get { return mName; } set { mName = value; } }

        [SerializeField]
        public string mDefineds = "OUTOF_UNITY";
        public string Defineds { get { return mDefineds; } set { mDefineds = value; } }

        [SerializeField]
        public string mOutPath = ".";
        public string OutPath { get { return mOutPath; } set { mOutPath = value; } }

        [SerializeField]
        public List<string> mSources = new List<string>();
        public List<string> Sources { get { return mSources; } set { mSources = value; } }

        [SerializeField]
        public List<string> mReferences = new List<string>();
        public List<string> References { get { return mReferences; } set { mReferences = value; } }

        public BundleInfo clone()
        {
            return new BundleInfo()
            {
                Name = this.Name,
                Defineds = this.Defineds,
                OutPath = this.OutPath,
                References = this.References,
            };
        }

        public bool mFoldOut = false;
        public bool mFoldOutSources = false;
        public bool mFoldOutReferences = false;
        public void DrawInspector(int indent)
        {
            EditorGUI.indentLevel += indent;
            mFoldOut = EditorGUILayout.Foldout(mFoldOut, Name, true);
            if (mFoldOut)
            {
                ++EditorGUI.indentLevel;
                mName = EditorGUILayout.TextField("Name", mName);
                OutPath = EditorGUILayout.TextField("OutPath", OutPath);
                Defineds = EditorGUILayout.TextField("Defineds", Defineds);

                var size = Sources.Count;
                EditorGUILayout.BeginHorizontal();
                {
                    mFoldOutSources = EditorGUILayout.Foldout(mFoldOutSources, "Sources", true);
                    size = EditorGUILayout.IntField(size);
                }
                EditorGUILayout.EndHorizontal();
                 
                if (size < Sources.Count)
                {
                    Sources.RemoveRange(size, Sources.Count - size);
                }
                else if (size > Sources.Count)
                {
                    for (var i = Sources.Count; i < size; ++i)
                        Sources.Add("");
                }

                if (mFoldOutSources)
                {
                    ++EditorGUI.indentLevel;
                    for(var i = 0; i<Sources.Count;++i)
                    {
                        Sources[i] = EditorGUILayout.TextField(Sources[i]);
                    }
                    --EditorGUI.indentLevel;
                }

                size = References.Count;
                EditorGUILayout.BeginHorizontal();
                {
                    mFoldOutReferences = EditorGUILayout.Foldout(mFoldOutReferences, "References", true);
                    size = EditorGUILayout.IntField(size);
                }
                EditorGUILayout.EndHorizontal();
                if (size < References.Count)
                {
                    References.RemoveRange(size, References.Count - size);
                }
                else if (size > References.Count)
                {
                    for (var i = References.Count; i < size; ++i)
                        References.Add("");
                }
                if (mFoldOutReferences)
                {
                    ++EditorGUI.indentLevel;
                    for(var i = 0; i<References.Count;++i)
                    {
                        References[i] = EditorGUILayout.TextField(References[i]);
                    }
                    --EditorGUI.indentLevel;
                }
                --EditorGUI.indentLevel;

                var rect = EditorGUILayout.GetControlRect();
                var sn = 4;
                var idx = -1;
                if(GUI.Button(rect.Split(++idx, sn), "Build"))
                {
                    Compile(this);
                }
                if(GUI.Button(rect.Split(++idx, sn), "Clone"))
                {
                    Instance().bundles.Insert(Instance().bundles.IndexOf(this) + 1, this.clone());
                }
                if(GUI.Button(rect.Split(++idx, sn), "Delete"))
                {
                    Instance().bundles.Remove(this);
                }

            }//if(mFoldOut)
            EditorGUI.indentLevel -= indent;
        }//DrawInspector
    }//class

    [HideInInspector]
    public List<BundleInfo> bundles;

    static void Compile(BundleInfo info)
    {
        AppLog.d("{1}/{0}",info.Name, info.OutPath);
        var msgs = EditorUtility.CompileCSharp(info.Sources.ToArray(), info.References.ToArray(), info.Defineds.Split(';'), info.OutPath);
        foreach (var msg in msgs)
        {
            AppLog.d(msg);
        }
    }

    public static string[] CompileCSharp(string[] sources, string[] references, string[] defines, string outputFile)
    {
        return EditorUtility.CompileCSharp(sources, references, defines, outputFile);
    }

    static DllCompile mInstance = null;
    [MenuItem("Tools/Create DllCompile.asset")]
    public static void Create()
    {
        mInstance = null;
        Instance();
    }

    static string BundleConfigAssetPath = "Assets/Editor/DllCompile.asset";
    public static DllCompile Instance()
    {
        if(mInstance == null)
        {
            mInstance = AssetDatabase.LoadAssetAtPath<DllCompile>(BundleConfigAssetPath);
            if(mInstance == null)
            {
                var dir = Path.GetDirectoryName(BundleConfigAssetPath);
                if(!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                mInstance = new DllCompile();
                AssetDatabase.CreateAsset(mInstance, BundleConfigAssetPath);
            }
        }

        return mInstance;
    }

    [CustomEditor(typeof(DllCompile))]
	public class Editor : UnityEditor.Editor
	{
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            var size = Instance().bundles.Count;
            EditorGUILayout.BeginHorizontal();
            {
//                mFoldOutSources = EditorGUILayout.Foldout(mFoldOutSources, "Sources", true);
                size = EditorGUILayout.IntField(size);
            }
            EditorGUILayout.EndHorizontal();

            if (size < Instance().bundles.Count)
            {
                Instance().bundles.RemoveRange(size, Instance().bundles.Count - size);
            }
            else if (size > Instance().bundles.Count)
            {
                for (var i = Instance().bundles.Count; i < size; ++i)
                    Instance().bundles.Add(new BundleInfo());
            }

            for (var i = 0; i < Instance().bundles.Count; ++i)
            {
                var item = Instance().bundles[i];
                item.DrawInspector(0);
            }
        }
	}
}