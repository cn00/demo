using System;
using System.IO;
using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Reflection;

[ExecuteInEditMode]
public class DllCompile : SingletonAsset<DllCompile>
{

    [Serializable]
    public class Config
    {
        public string Name;
        // [SerializeField]
        // public string mName = ".dll";
        // public string Name { get { return mName; } set { mName = value; } }

        [SerializeField]
        public List<string> Defineds = new List<string>()
        {
            "UNITY_DLL",
        };
        public bool KeepMdb = false;

        public string OutPath = ".";

        public string SourceDir = "";

        [SerializeField]
        public List<string> LibSearchingPath = new List<string>(){
            "MonoBleedingEdge/lib/mono/4.5/",
            "Managed/",
        };

        [SerializeField]
        public List<string> References = new List<string>()
        {
            "UnityEngine.dll",
            "UnityEditor.dll"
        };

        public Config clone()
        {
            return new Config()
            {
                Name = this.Name + "-copy",
                Defineds = this.Defineds,
                OutPath = this.OutPath,
                SourceDir = this.SourceDir,
                References = this.References,
            };
        }

        public void DrawInspector(int indent = 0, GUILayoutOption[] guiOpts = null)
        {
            // if (Foldout)
            {
                // base.DrawInspector(indent);

                var rect = EditorGUILayout.GetControlRect();
                var sn = 3;
                var idx = -1;
                if (GUI.Button(rect.Split(++idx, sn), "Build"))
                {
                    Build(this);
                }
                if (GUI.Button(rect.Split(++idx, sn), "Clone"))
                {
                    Instance().bundles.Insert(Instance().bundles.IndexOf(this) + 1, this.clone());
                }
                if (GUI.Button(rect.Split(++idx, sn), "Delete"))
                {
                    Instance().bundles.Remove(this);
                }
                // if (GUI.Button(rect.Split(++idx, sn), "Copy"))
                // {
                //     Instance().bundles.Insert(Instance().bundles.IndexOf(this), this.clone());
                // }
            }//if(mFoldout)
        }//DrawInspector
    }//class

    [HideInInspector]
    public List<Config> bundles = new List<Config>();

    [HideInInspector]
    public UnityEngine.Object PathGetterObj;

    static void Build(Config info)
    {
        AppLog.d("{0} => {1}", info.Name, info.OutPath);
        var sources = Directory.GetFiles(info.SourceDir, "*.cs", SearchOption.AllDirectories);

        var reference = new List<string>(); //info.References.Clone();
#if UNITY_EDITOR_OSX
        var baseDir = AppDomain.CurrentDomain.BaseDirectory + "/Unity.app/Contents/";
#elif UNITY_EDITOR_WIN
        var baseDir = AppDomain.CurrentDomain.BaseDirectory + "/Data/";
#endif
        foreach (var i in info.LibSearchingPath)
        {
            foreach (var j in info.References)
            {
                var path = baseDir + i + j;
                if(File.Exists(path))
                {
                    reference.Add(path);
                    AppLog.d(path);
                    break;
                }
            }
        } 

        // var monoversion = PlayerSettings.scriptingRuntimeVersion;
        // var apilevel = PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup);
        var msgs = EditorUtility.CompileCSharp(sources, reference.ToArray(), info.Defineds.ToArray(), info.OutPath);

        foreach (var msg in msgs)
        {
            AppLog.d("CompileCSharp: " + msg);
        }
        if(!info.KeepMdb)
            File.Delete(info.OutPath + ".mdb");
        AssetDatabase.ImportAsset(info.OutPath);
    }

    [MenuItem("Tools/Create/DllCompile.asset")]
    public static void Create()
    {
        mInstance = null;
        Instance();
    }

    [CustomEditor(typeof(DllCompile))]
    public class Editor : UnityEditor.Editor
    {
        bool Foldout = false;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            // var obj = (Instance().bundles as object);
            Inspector.DrawList("DllConfigs", Instance().bundles, ref Foldout);

            Instance().DrawSaveButton();

            Instance().PathGetterObj = EditorGUILayout.ObjectField("PathGetter", Instance().PathGetterObj, typeof(UnityEngine.Object), true);
            if (Instance().PathGetterObj != null)
            {
                var tmpDir = AssetDatabase.GetAssetPath(Instance().PathGetterObj.GetInstanceID());
                EditorGUILayout.SelectableLabel(tmpDir);
            }
#if UNITY_EDITOR_OSX
                var distDir = AppDomain.CurrentDomain.BaseDirectory + "/Unity.app/Contents/Resources/ScriptTemplates/89-LuaScript-NewLuaScript.lua.txt";
#elif UNITY_EDITOR_WIN
                var distDir = AppDomain.CurrentDomain.BaseDirectory + "/Data/Resources/ScriptTemplates/89-LuaScript-NewLuaScript.lua.txt";
#endif
            EditorGUILayout.SelectableLabel(distDir);
            var rect = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect, "Copy/Update Lua Script Template to Editor"))
            {
                // File.Copy("Assets/doc/87-LuaScript-NewLuaScript.lua.txt", distDir, true);
                var s = File.ReadAllText("Assets/doc/87-LuaScript-NewLuaScript.lua.txt");
                s = s.Replace("-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt", "");
                File.WriteAllText(distDir, s);
            }

            // //BaseDirectory: /Applications/Unity-2017.4.1f1
            // EditorGUILayout.SelectableLabel("BaseDirectory: "+ AppDomain.CurrentDomain.BaseDirectory);

            // //Location: /Users/a3/.jenkins/workspace/unity_test/Library/ScriptAssemblies/Assembly-CSharp-Editor.dll
            // EditorGUILayout.SelectableLabel("Location: " + System.Reflection.Assembly.GetExecutingAssembly().Location);

            // //CodeBase: file:///Users/a3/.jenkins/workspace/unity_test/Library/ScriptAssemblies/Assembly-CSharp-Editor.dll
            // EditorGUILayout.SelectableLabel("CodeBase: " + System.Reflection.Assembly.GetExecutingAssembly().CodeBase);

        }
    }
}