using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Net;

#if UNITY_EDITOR
using BundleManifest = System.Collections.Generic.List<BuildConfig.GroupInfo>;
using GroupInfo = BuildConfig.GroupInfo;
using BundleInfo = BuildConfig.BundleInfo;

namespace UnityEditor
{
    #region CustomEditor

    [CustomEditor(typeof(BuildConfig))]
    public class BuildConfigEditor : UnityEditor.Editor
    {
        private const string Tag = "BuildConfigEditor";
        bool allInclude = false;
        bool allRebuild = false;
        bool showBundles = false;
        bool showBuilds = false;
        bool showConfigs = false;
        bool showXLuaConfig = false;
        List<string> XLuaConfigUnityEngine;

        BuildConfig mTarget = null;

        public void OnEnable()
        {
            mTarget = target as BuildConfig;
            // XLuaConfigUnityEngine = XLuaGenConfig.LuaCallCSharpUnityEngine.Select(t => t.FullName).ToList();
        }

        void Refresh()
        {
            mTarget.Ip = BuildConfig.LocalIpAddress();

            var newGroups = new BundleManifest();
            var groups = Directory.GetDirectories(BuildConfig.BundleResRoot, "*", SearchOption.TopDirectoryOnly);
            int n = 0;
            foreach (var group in groups)
            {
                EditorUtility.DisplayCancelableProgressBar("update group ...", group, (float) (++n) / groups.Length);

                var groupName = group.upath().Replace(BuildConfig.BundleResRoot, "");
                GroupInfo groupInfo = mTarget.Groups.Find(i => i.Name == groupName);
                if (groupInfo == null)
                {
                    groupInfo = new GroupInfo()
                    {
                        Name = groupName,
                        Bundles = new List<BundleInfo>(),
                        mRebuild = true,
                    };
                }

                var newBundles = new List<BundleInfo>();
                foreach (var bundle in Directory.GetDirectories(group, "*", SearchOption.TopDirectoryOnly))
                {
                    var bundlePath = bundle.upath();
                    var bundleName = bundlePath.Replace(BuildConfig.BundleResRoot, "") + BuildConfig.BundlePostfix;
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

                    foreach (var f in Directory.GetFiles(bundle, "*", SearchOption.AllDirectories)
                        .Where(i => !i.EndsWith(".meta")))
                    {
                        var assetImporter = AssetImporter.GetAtPath(f);
                        if (assetImporter != null)
                        {
                            assetImporter.assetBundleName = bundleName; //"";//
                            var assetTimeStamp = assetImporter.assetTimeStamp;
                        }
                    }

                    // if (time > 0)
                    newBundles.Add(bundleInfo);
                } //for 2

                //            AssetDatabase.GetAllAssetBundleNames();
                // AssetDatabase.RemoveUnusedAssetBundleNames();

                groupInfo.Bundles = newBundles;
                groupInfo.Refresh();

                if (groupInfo.Bundles.Count > 0)
                    newGroups.Add(groupInfo);
            } //for 1

            mTarget.Groups = newGroups;

            EditorUtility.ClearProgressBar();
        }

        public static IEnumerator Execute(string exe, string prmt
            , System.Diagnostics.DataReceivedEventHandler OutputDataReceived = null
            , Action end = null
            , float total = 0, string processingtag = "bash", string info = ""
        )
        {
            bool finished = false;
            var process = new System.Diagnostics.Process();
            var processing = 0f;
            try
            {
                // UnityEngine.Debug.Log(exe + " " + prmt);
                var pi = new System.Diagnostics.ProcessStartInfo(exe, prmt);
                pi.WorkingDirectory = ".";
                pi.RedirectStandardInput = false;
                pi.RedirectStandardOutput = true;
                pi.RedirectStandardError = true;
                pi.UseShellExecute = false;
                pi.CreateNoWindow = true;

                if (OutputDataReceived != null)
                    process.OutputDataReceived += OutputDataReceived;
                process.OutputDataReceived += (sender, e) =>
                {
                    if (string.IsNullOrEmpty(e.Data))
                        return;
                    if (e.Data.StartsWith(processingtag))
                        ++processing;
                    // UnityEngine.Debug.Log(e.Data);
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        UnityEngine.Debug.LogError(e.GetType() + ": " + e.Data);
                };
                process.Exited += (object sender, EventArgs e) =>
                {
                    finished = true;
                    UnityEngine.Debug.Log("Exit");
                };

                process.StartInfo = pi;
                process.EnableRaisingEvents = true;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                // process.WaitForExit();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("catch: " + e);
            }

            while (!finished)
            {
                if (total > 1)
                {
                    EditorUtility.DisplayCancelableProgressBar("uploading ...", info + ": " + processing + "/" + total,
                        processing / total);
                }

                yield return null;
            }

            if (end != null)
                end();

            // UnityEngine.Debug.Log("finished: " + process.ExitCode);
            EditorUtility.ClearProgressBar();
            yield return null;
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
                    if (luamono.mAsset == null) //|| luamono.LuaScript.Text == null)
                    {
                        AppLog.w(Tag, p + " luamono.luaScript not set");
                        continue;
                    }

                    // var txtpath = tpath + ".txt";
                    // var txtasset = AssetDatabase.LoadAssetAtPath<TextAsset>(txtpath);
                    {

                        var tpath = AssetDatabase.GetAssetPath(luamono.mAsset);
                        luamono.luaPath = tpath.Remove(tpath.Length - 4).Replace(BuildConfig.BundleResRoot, "");
                        // tpath = tpath.Remove(tpath.Length - 4).Replace(BuildConfig.BundleResRoot, "");
                        // if(luamono.LuaScript.path != tpath)
                        // {
                        //     luamono.LuaScript.path = tpath;
                        //     AppLog.d("FixPrefabLuaPath", p);
                        // }
                        EditorUtility.SetDirty(prefab);
                    }
                }

                if (luamonos.Count() > 0)
                {
                    ++count;
                    PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.Default);
                }

                MonoBehaviour.DestroyImmediate(go);
                EditorUtility.DisplayCancelableProgressBar("FixPrefabLuaPath ..."
                    , count + "/" + prefabPaths.Count() + i, (float) (count) / prefabPaths.Count());
                if (count % 10 == 0)
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

            }

            // clean
            GUILayout.Space(1f);
            {
                var rect = EditorGUILayout.GetControlRect();
                var sn = 5;
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

            // update prefab lua path
            GUILayout.Space(1f);
            {
                var rect = EditorGUILayout.GetControlRect();
                var sn = 4;
                var idx = -1;
                if (GUI.Button(rect.Split(++idx, sn), "Refresh"))
                {
                    Refresh();
                }

                if (GUI.Button(rect.Split(++idx, sn), "FixPrefabLuaPath"))
                {
                    EditorCoroutine.StartCoroutine(FixPrefabLuaPath());
                }
            }


            EditorGUILayout.LabelField("LastBuildTime",
                DateTime.FromFileTime(mTarget.LastBuildTime).ToString("yyyy/MM/dd HH:mm:ss"));
            mTarget.ForceRebuild = EditorGUILayout.Toggle("ForceRebuild", mTarget.ForceRebuild, guiOpts);
            mTarget.BuildScene = EditorGUILayout.Toggle("BuildScene", mTarget.BuildScene, guiOpts);

            mTarget.BundleBuildOptions =
                (BuildAssetBundleOptions) EditorGUILayout.EnumFlagsField("BundleBuildOptions",
                    mTarget.BundleBuildOptions);

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

            mTarget.runInBackground = PlayerSettings.runInBackground =
                EditorGUILayout.Toggle("runInBackground", mTarget.runInBackground);

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
                    mTarget.Ip = EditorGUILayout.TextField(mTarget.Ip);
                    mTarget.Port = EditorGUILayout.TextField(mTarget.Port);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                using (var verticalScope2 = new EditorGUILayout.VerticalScope("box"))
                {
                    DrawBundleConfig(guiOpts);
                    verticalScope2.Dispose();
                }
            }

            //apk ios.proj exe app etc.
            Inspector.DrawList("Channels", mTarget.Channels, ref showBuilds, false, item =>
            {
                var i = item as ChannelConfig;
                if (string.IsNullOrEmpty(i.Name))
                {
                    i.Name = i.Channel + ":" + (int) i.Channel;
                }
            });

            Inspector.DrawList("Configs", mTarget.Configs, ref showConfigs, false, null);

            // Inspector.DrawList("XLuaConfig", XLuaConfigUnityEngine, ref showXLuaConfig);

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

            EditorCoroutine.StartCoroutine(BuildScript.BuildAssetBundle(target, rebuild, () =>
            {
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
}
#endif //UNITY_EDITOR


