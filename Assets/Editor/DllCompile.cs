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

[ExecuteInEditMode]
public class DllCompile : SingletonAsset<DllCompile>
{

    [Serializable]
    public class Config : InspectorDraw
    {
        // [SerializeField]
        // public string mName = ".dll";
        // public string Name { get { return mName; } set { mName = value; } }

        public string Defineds = "UNITY_DLL";

        public string OutPath = ".";

        public string SourceDir = "";

        [SerializeField]
        public List<string> References = new List<string>();

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

        public override void DrawInspector(int indent = 0, GUILayoutOption[] guiOpts = null)
        {
            if (FoldOut)
            {
                base.DrawInspector(indent);

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
            }//if(mFoldOut)
        }//DrawInspector
    }//class

    [HideInInspector]
    public List<Config> bundles = new List<Config>();

    [HideInInspector]
    public UnityEngine.Object PathGetterObj;

    static void Build(Config info)
    {
        AppLog.d("{0}: {1}", info.Name, info.OutPath);
        var sources = Directory.GetFiles(info.SourceDir, "*.cs", SearchOption.AllDirectories);
        var reference = info.References.Clone();
#if UNITY_MACOSX
        reference.Add(AppDomain.CurrentDomain.BaseDirectory + "/Unity.app/Contents/Managed/UnityEngine.dll");
        reference.Add(AppDomain.CurrentDomain.BaseDirectory + "/Unity.app/Contents/Managed/UnityEditor.dll");
#elif UNITY_WIN32
        reference.Add(AppDomain.CurrentDomain.BaseDirectory + "/Data/Managed/UnityEngine.dll");
        reference.Add(AppDomain.CurrentDomain.BaseDirectory + "/Data/Managed/UnityEditor.dll");
#endif
        var msgs = EditorUtility.CompileCSharp(sources, reference.ToArray(), info.Defineds.Split(';'), info.OutPath);
        foreach (var msg in msgs)
        {
            AppLog.d(msg);
        }
        File.Delete(info.OutPath + ".mdb");
        AssetDatabase.ImportAsset(info.OutPath);
    }

    [MenuItem("Tools/Create DllCompile.asset")]
    public static void Create()
    {
        mInstance = null;
        Instance();
    }

    [CustomEditor(typeof(DllCompile))]
    public class Editor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            DrawListCount(Instance().bundles);
            for (var i = 0; i < Instance().bundles.Count; ++i)
            {
                var item = Instance().bundles[i];
                if(item != null)
                    item.Draw();
            }
            Instance().DrawSaveButton();

            Instance().PathGetterObj = EditorGUILayout.ObjectField("PathGetter", Instance().PathGetterObj, typeof(UnityEngine.Object), true);
            if (Instance().PathGetterObj != null)
            {
                var tmpDir = AssetDatabase.GetAssetPath(Instance().PathGetterObj.GetInstanceID());
                EditorGUILayout.SelectableLabel(tmpDir);
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